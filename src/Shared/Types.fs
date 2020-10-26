module Shared.Types

open System

type Rule =
    { Id: Guid
      Name: string
      ApplicableOn: DayOfWeek list }

type MealCategory = { Id: Guid; Name: string }

type Meal =
    { Id: Guid
      Name: string
      Category: MealCategory option
      Rules: Rule list }

type MealOptions =
    { DaysBetweenSameMeal: int
      DaysBetweenSameMealCategory: int
      DaysToCalculate: int
      FromDate: DateTime }

module Route =
    let builder typeName methodName = sprintf "/api/%s/%s" typeName methodName

type IAnonymousApi =
    { IsAuthenticated: unit -> Async<bool> }

type IMealApi =
    { GetMeals: unit -> Async<Meal list>
      GetMeal: Guid -> Async<Meal option>
      AddMeal: Meal -> Async<unit>
      EditMeal: Meal -> Async<unit>
      GetRules: unit -> Async<Rule list>
      AddRule: Rule -> Async<unit>
      GetDaysOfWeek: unit -> Async<DayOfWeek list>
      AddCategory: MealCategory -> Async<unit>
      GetCategories: unit -> Async<MealCategory list> }
