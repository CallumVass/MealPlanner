module Storage

open Shared.Types
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
      CategoryName: string option
      Name: string }

[<CLIMutable>]
type private MealRuleEntity =
    { Id: Guid
      Name: string
      DayOfWeek: string }

let private toDayOfWeek dayOfWeek = Enum.Parse<DayOfWeek>(dayOfWeek)

let private toDayOfWeekString dayOfWeek =
    Enum.GetName(typeof<DayOfWeek>, dayOfWeek)

let private toRule mealRule =
    let ((id, name), mealRules) = mealRule

    { Id = id
      Name = name
      ApplicableOn =
          mealRules
          |> Seq.map (fun m -> m.DayOfWeek)
          |> Seq.map toDayOfWeek
          |> List.ofSeq }

let private toRules mealRules =
    mealRules
    |> Seq.groupBy (fun r -> r.Id, r.Name)
    |> Seq.map toRule

let private toDomain (meal: MealEntity) (mealRules: MealRuleEntity seq) =
    { Id = meal.Id
      Name = meal.Name
      CategoryName = meal.CategoryName
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

let private insertRuleDay connection (ruleId: Guid) dayOfWeek =
    let sql = """
    INSERT INTO RuleRuleDays (RuleId, DayOfWeek)
    VALUES (@ruleId, @dayOfWeek)
    """

    async {

        let dayOfWeekString = dayOfWeek |> toDayOfWeekString

        let! _ =
            execute
                connection
                sql
                !{| ruleId = ruleId
                    dayOfWeek = dayOfWeekString |}

        return ()
    }

let private insertRuleDays connection applicableOn ruleId =
    async {
        let! _ =
            applicableOn
            |> Seq.asyncMap (insertRuleDay connection ruleId)

        return ()
    }

let private insertMealRule connection (mealId: Guid) (rule: Rule) =
    let sql = """
    INSERT INTO MealRules (MealId, RuleId)
    VALUES (@mealId, @ruleId)
    """

    async {
        let! _ = execute connection sql !{| ruleId = rule.Id; mealId = mealId |}

        return ()
    }

let private insertMealRules connection (rules: Rule seq) mealId =
    async {
        let! _ =
            rules
            |> Seq.asyncMap (insertMealRule connection mealId)

        return ()
    }

let getMeals connectionString userId =
    let sql = """
    SELECT m.Id, m.Name, c.Name AS CategoryName
        FROM Meals m
    LEFT JOIN MealCategories c ON m.CategoryId = c.Id
    WHERE m.UserId = @userId
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
    INSERT INTO Meals (Name, UserId)
    OUTPUT Inserted.Id
    VALUES (@name, @userId)
    """

    async {
        use! connection = getConnection connectionString
        let! mealId = querySingle connection sql !{| userId = userId; name = meal.Name |}

        let! _ =
            mealId
            |> Option.asyncApply (insertMealRules connection meal.Rules)

        return ()
    }

let editMealRules (meal: Meal) connection =
    let sql = """
    DELETE FROM MealRules WHERE MealId = @mealId
    """

    async {
        let! _ = execute connection sql !{| mealId = meal.Id |}
        let! _ = meal.Id |> insertMealRules connection meal.Rules

        return ()
    }

let editMeal connectionString (meal: Meal) userId =

    let sql = """
    UPDATE Meals SET Name = @name WHERE Id = @id AND UserId = @userId
    """

    async {
        use! connection = getConnection connectionString

        let! _ =
            execute
                connection
                sql
                !{| name = meal.Name
                    id = meal.Id
                    userId = userId |}

        let! _ = connection |> editMealRules meal

        return ()
    }

let addRule connectionString (rule: Rule) userId =
    let sql = """
    INSERT INTO Rules (Name, UserId)
    OUTPUT Inserted.Id
    VALUES (@name, @userId)
    """

    async {
        use! connection = getConnection connectionString
        let! ruleId = querySingle connection sql !{| userId = userId; name = rule.Name |}

        let! _ =
            ruleId
            |> Option.asyncApply (insertRuleDays connection rule.ApplicableOn)

        return ()
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

let getDaysOfWeek connectionString =
    let sql = """
    SELECT DayOfWeek
        FROM RuleDays
    """

    async {
        use! connection = getConnection connectionString
        let! result = query connection sql None

        return result
               |> Seq.map toDayOfWeek
               |> Seq.sort
               |> List.ofSeq
    }

type MealStorage(config: IConfiguration) =
    let connectionString =
        config.GetConnectionString("MealPlanner")

    member __.GetMeals userId = userId |> getMeals connectionString

    member __.GetMeal mealId userId =
        userId |> getMeal connectionString mealId

    member __.AddMeal meal userId = userId |> addMeal connectionString meal

    member __.EditMeal meal userId = userId |> editMeal connectionString meal

    member __.GetRules userId = userId |> getRules connectionString

    member __.AddRule rule userId = userId |> addRule connectionString rule

    member __.GetDaysOfWeek = connectionString |> getDaysOfWeek
