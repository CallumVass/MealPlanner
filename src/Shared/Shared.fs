namespace Shared

open System

type Rule =
    { Id: Guid
      Name: string
      ApplicableOn: DayOfWeek list }

type Meal =
    { Id: Guid
      Name: string
      Rules: Rule list }

type MealOptions =
    { DaysBetweenSameMeal: int
      DaysToCalculate: int }

module Route =
    let builder typeName methodName = sprintf "/api/%s/%s" typeName methodName

type IMealApi =
    { GetMeals: unit -> Async<Result<Meal list, string>> }
