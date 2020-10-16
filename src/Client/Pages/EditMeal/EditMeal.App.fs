module EditMeal.App

open Elmish
open EditMeal.Types
open Form

let defaultState mealId =
    { MealId = mealId
      Meal = HasNotStartedYet }

let init mealId =
    defaultState mealId, Cmd.ofMsg (GetMeal Started)

let private resolveForm fn f state =
    let form = f |> fn
    { state with
          Meal = Resolved(Some form) }

let private updateResolvedMealState meal fn state =
    match meal with
    | Some meal -> state |> resolveForm fn meal, Cmd.none
    | None -> { state with Meal = Resolved None }, Cmd.none

let update msg state =
    match msg with
    | GetMeal Started ->
        let loadMeal =
            async {
                let! meal = Api.mealApi.GetMeal(state.MealId)
                return GetMeal(Finished meal)
            }

        { state with Meal = InProgress }, Cmd.fromAsync loadMeal
    | GetMeal (Finished meal) ->
        state
        |> updateResolvedMealState meal ValidatedForm.init
    | FormChanged f ->
        match state.Meal with
        | Resolved m ->
            state
            |> updateResolvedMealState m (f |> ValidatedForm.updateWith)
        | _ -> state, Cmd.none
    | Save meal ->
        printfn "%A" meal

        state, Cmd.none
