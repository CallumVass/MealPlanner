module Index.Logic

open Shared
open System
open Index.Types

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

let private getMeal currentMeals dayOfWeek state =
    let mealsWithoutRules =
        state.AvailableMeals
        |> List.filter filterMealsWithoutRules

    let meals =
        state.AvailableMeals
        |> List.filter (filterByDayOfWeek dayOfWeek)

    let applicableMeals = mealsWithoutRules @ meals

    let filteredMeals =
        applicableMeals
        |> List.filter (filterByHaveHadMealRecently currentMeals state.Options.DaysBetweenSameMeal)

    filteredMeals
    |> List.item (rnd.Next filteredMeals.Length)

let private createMeal currentMeals index state =
    let date = DateTime.Now.Date.AddDays(float index)

    let day =
        Enum.GetName(typeof<DayOfWeek>, date.DayOfWeek)

    let meal =
        getMeal currentMeals date.DayOfWeek state

    day, date.ToString("dd/MM/yyyy"), meal

let calculate state =
    if (state.AvailableMeals.IsEmpty) then
        state
    else
        let meals =
            seq { for i in 1 .. state.Options.DaysToCalculate -> i }
            |> List.ofSeq
            |> List.fold (fun currentMeals index ->
                currentMeals
                @ [ createMeal currentMeals index state ]) []

        { state with ChosenMeals = meals }
