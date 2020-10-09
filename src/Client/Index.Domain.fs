module Index.Domain

open Shared
open System

type AuthenticatedUser =
    { AvailableMeals: Meal list
      ChosenMeals: (string * string * Meal) list }

type UserData =
    | Unauthenticated
    | Authenticated of AuthenticatedUser

type Domain =
    { Options: MealOptions
      UserData: UserData }

let rnd = Random()

let private filterByDayOfWeek dayOfWeek meal =
    meal.Rules
    |> List.collect (fun m -> m.ApplicableOn)
    |> List.contains dayOfWeek

let private filterByHaveHadMealRecently currentMeals daysBetweenSameMeal meal =

    currentMeals
    |> List.rev
    |> List.truncate daysBetweenSameMeal
    |> List.exists (fun (_, _, m) -> m = meal)
    |> not

let private filterMealsWithoutRules meal = meal.Rules.Length = 0

let private getMeal currentMeals dayOfWeek model options =

    let mealsWithoutRules =
        model.AvailableMeals
        |> List.filter filterMealsWithoutRules

    let meals =
        model.AvailableMeals
        |> List.filter (filterByDayOfWeek dayOfWeek)
        |> List.filter (filterByHaveHadMealRecently currentMeals options.DaysBetweenSameMeal)

    let applicableMeals = mealsWithoutRules @ meals

    applicableMeals
    |> List.item (rnd.Next applicableMeals.Length)

let private createMeal currentMeals index model options =
    let date = DateTime.Now.Date.AddDays(float index)

    let day =
        Enum.GetName(typeof<DayOfWeek>, date.DayOfWeek)

    let meal =
        getMeal currentMeals date.DayOfWeek model options

    day, date.ToString("dd/MM/yyyy"), meal

let calculate model options =

    if (model.AvailableMeals.IsEmpty) then
        model
    else
        let meals =
            seq { for i in 1 .. options.DaysToCalculate -> i }
            |> List.ofSeq
            |> List.fold (fun currentMeals index ->
                currentMeals
                @ [ createMeal currentMeals index model options ]) []

        { model with ChosenMeals = meals }

let applyMeals model maybeMeals =
    match maybeMeals with
    | Ok meals ->
        { model with
              UserData =
                  Authenticated
                      ({ AvailableMeals = meals
                         ChosenMeals = [] }) }
    | Error _ ->
        { model with
              UserData = Unauthenticated }
