module Shared.Types

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

type IAnonymousApi =
    { IsAuthenticated: unit -> Async<bool> }

type IMealApi =
    { GetMeals: unit -> Async<Meal list>
      GetMeal: Guid -> Async<Meal option>
      AddMeal: Meal -> Async<unit>

      GetRules: unit -> Async<Rule list>
      AddRule: Rule -> Async<unit> }