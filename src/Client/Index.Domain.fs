module Index.Domain

open Shared
open System

type AuthenticatedUser =
    { Options: MealOptions
      AvailableMeals: Meal list
      ChosenMeals: (string * string * Meal) list }

type UserData =
    | Unauthenticated
    | Authenticated of AuthenticatedUser

type Domain = { UserData: UserData }

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

let private getMeal currentMeals dayOfWeek model =

    let mealsWithoutRules =
        model.AvailableMeals
        |> List.filter filterMealsWithoutRules

    let meals =
        model.AvailableMeals
        |> List.filter (filterByDayOfWeek dayOfWeek)

    let applicableMeals = mealsWithoutRules @ meals

    let filteredMeals =
        applicableMeals
        |> List.filter (filterByHaveHadMealRecently currentMeals model.Options.DaysBetweenSameMeal)

    filteredMeals
    |> List.item (rnd.Next filteredMeals.Length)

let private createMeal currentMeals index model =
    let date = DateTime.Now.Date.AddDays(float index)

    let day =
        Enum.GetName(typeof<DayOfWeek>, date.DayOfWeek)

    let meal =
        getMeal currentMeals date.DayOfWeek model

    day, date.ToString("dd/MM/yyyy"), meal

let calculate model =

    if (model.AvailableMeals.IsEmpty) then
        model
    else
        let meals =
            seq { for i in 1 .. model.Options.DaysToCalculate -> i }
            |> List.ofSeq
            |> List.fold (fun currentMeals index ->
                currentMeals
                @ [ createMeal currentMeals index model ]) []

        { model with ChosenMeals = meals }
