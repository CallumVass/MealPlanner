module Storage

open Shared
open System
open DbUp
open Microsoft.Extensions.Configuration
open System.Reflection

type Migrator(config: IConfiguration) =
    member __.Migrate() =
        let connectionString =
            config.GetConnectionString("MealPlanner")

        let upgrader =
            DeployChanges.To.SqlDatabase(connectionString)
                         .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly()).LogToAutodetectedLog().Build()

        upgrader.PerformUpgrade() |> ignore

[<CLIMutable>]
type private MealEntity =
    { Id: Guid
      UserId: string
      Name: string }

[<CLIMutable>]
type private MealRuleEntity =
    { Id: Guid
      Name: string
      DayOfWeek: string }

let private toDayOfWeek mealRule =
    Enum.Parse<DayOfWeek>(mealRule.DayOfWeek)

let private toRule mealRule =
    let ((id, name), mealRules) = mealRule

    { Id = id
      Name = name
      ApplicableOn = mealRules |> Seq.map toDayOfWeek |> List.ofSeq }

let private toRules mealRules =
    mealRules
    |> Seq.groupBy (fun r -> r.Id, r.Name)
    |> Seq.map toRule

let private toDomain (meal: MealEntity) (mealRules: MealRuleEntity seq) =
    { Id = meal.Id
      Name = meal.Name
      Rules = (toRules mealRules) |> List.ofSeq }

let private getRulesForMeal connection (meal: MealEntity) =
    let sql = """
    SELECT DISTINCT r.Id, r.Name, rd.DayOfWeek
        FROM Rules r
	INNER JOIN MealRules mr ON r.Id = mr.RuleId
	INNER JOIN RuleRuleDays rd ON r.Id = rd.RuleId
    WHERE mr.MealId = @mealId
    """

    async {
        let! result = query connection sql !{| mealId = meal.Id |}
        return result |> (toDomain meal)
    }

let getMeals connectionString userId =
    let sql = """
    SELECT Id, Name
        FROM Meals
    WHERE UserId = @userId
    """

    async {
        use! connection = getConnection connectionString
        let! result = query connection sql !{| userId = userId |}

        let! meals =
            result
            |> Seq.asyncMap (getRulesForMeal connection)

        return meals |> List.ofSeq
    }

let getMeal connectionString mealId userId =
    let sql = """
    SELECT Id, Name
        FROM Meals
    WHERE UserId = @userId AND Id = @mealId
    """

    async {
        use! connection = getConnection connectionString
        let! result = querySingle connection sql !{| userId = userId; mealId = mealId |}

        return! result
                |> Option.asyncApply (getRulesForMeal connection)
    }

let addMeal connectionString (meal: Meal) userId =
    let sql = """
    INSERT INTO Meals (Name, UserId) VALUES (@name, @userId)
    """

    async {
        use! connection = getConnection connectionString
        let! result = execute connection sql !{| userId = userId; name = meal.Name |}

        return result
    }

let addRule connectionString (rule: Rule) userId =
    let sql = """
    INSERT INTO Rules (Name, UserId) VALUES (@name, @userId)
    """

    async {
        use! connection = getConnection connectionString
        let! result = execute connection sql !{| userId = userId; name = rule.Name |}

        return result
    }

let private getRules connectionString userId =
    let sql = """
    SELECT DISTINCT r.Id, r.Name, rd.DayOfWeek
        FROM Rules r
	INNER JOIN RuleRuleDays rd ON r.Id = rd.RuleId
    WHERE r.UserId = @userId
    """

    async {
        use! connection = getConnection connectionString
        let! result = query connection sql !{| userId = userId |}
        return result |> toRules |> List.ofSeq
    }

type MealStorage(config: IConfiguration) =
    let connectionString =
        config.GetConnectionString("MealPlanner")

    member __.GetMeals userId = userId |> getMeals connectionString

    member __.GetMeal mealId userId =
        userId |> getMeal connectionString mealId

    member __.AddMeal meal userId = userId |> addMeal connectionString meal

    member __.GetRules userId = userId |> getRules connectionString

    member __.AddRule rule userId = userId |> addRule connectionString rule
