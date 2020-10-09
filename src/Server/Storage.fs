module Storage

open Shared
open System
open DbUp
open Microsoft.Extensions.Configuration
open System.Reflection

module Async =
    let map f workflow =
        async {
            let! res = workflow
            return f res
        }

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
type private MealAndRuleEntity =
    { Id: Guid
      Name: string
      DayOfWeek: string
      MealId: Guid }

let private toDomain (meal: MealEntity) (mealRules: MealAndRuleEntity seq) =
    { Id = meal.Id
      Name = meal.Name
      Rules = [] }

let private getRules connection (meal: MealEntity) =
    let sql = """
    SELECT DISTINCT r.Id, r.Name, rd.DayOfWeek, mr.MealId
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
            |> Seq.map (getRules connection)
            |> Async.Parallel

        return meals |> List.ofSeq
    }

type MealStorage(config: IConfiguration) =
    let connectionString =
        config.GetConnectionString("MealPlanner")

    member __.GetMeals userId = getMeals connectionString userId
