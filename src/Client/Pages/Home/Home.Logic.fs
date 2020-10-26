module Home.Logic

open Shared.Types
open System
open Home.Types

let rnd = Random()

let private filterByDayOfWeek dayOfWeek meal =
    meal.Rules
    |> List.collect (fun m -> m.ApplicableOn)
    |> List.contains dayOfWeek

let private filterByHaveHadMealRecently currentMeals daysBetweenSameMeal meal =

    let filterMeal meal m =
        match m with
        | Some m -> m = meal
        | None -> false

    currentMeals
    |> List.rev
    |> List.truncate daysBetweenSameMeal
    |> List.exists (fun (_, _, m) -> m |> filterMeal meal)
    |> not

let private filterByHaveHadMealCategoryRecently currentMeals daysBetweenSameMealCategory meal =

    let filterByCategory category mealCategory =
        (category, mealCategory)
        ||> Option.map2 (=)
        |> Option.defaultValue false

    let filterMeal meal m =
        m
        |> Option.map (fun m -> m.Category |> filterByCategory meal.Category)
        |> Option.defaultValue false

    currentMeals
    |> List.rev
    |> List.truncate daysBetweenSameMealCategory
    |> List.exists (fun (_, _, m) -> m |> filterMeal meal)
    |> not

let private filterMealsWithoutRules meal = meal.Rules.Length = 0

let private getMeal currentMeals dayOfWeek options meals =
    let mealsWithoutRules =
        meals |> List.filter filterMealsWithoutRules

    let meals =
        meals |> List.filter (filterByDayOfWeek dayOfWeek)

    let applicableMeals = mealsWithoutRules @ meals

    let filteredMeals =
        applicableMeals
        |> List.filter (filterByHaveHadMealRecently currentMeals options.DaysBetweenSameMeal)
        |> List.filter (filterByHaveHadMealCategoryRecently currentMeals options.DaysBetweenSameMealCategory)

    filteredMeals
    |> List.tryItem (rnd.Next filteredMeals.Length)

let private createMeal currentMeals index options meals =
    let date = options.FromDate.AddDays(float index)

    let day =
        Enum.GetName(typeof<DayOfWeek>, date.DayOfWeek)

    let meal =
        meals
        |> getMeal currentMeals date.DayOfWeek options

    day, date.ToString("dd/MM/yyyy"), meal

let calculate state =
    match state.AvailableMeals with
    | Resolved meals ->
        if (meals.IsEmpty) then
            state
        else
            let meals =
                seq { for i in 0 .. (state.Options.FormData.DaysToCalculate - 1) -> i }
                |> List.ofSeq
                |> List.fold (fun currentMeals index ->
                    currentMeals
                    @ [ meals
                        |> createMeal currentMeals index state.Options.FormData ]) []

            { state with ChosenMeals = meals }
    | _ -> state
