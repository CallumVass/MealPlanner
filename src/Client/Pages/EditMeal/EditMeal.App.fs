module EditMeal.App

open Elmish
open EditMeal.Types

let defaultState mealId =
    { MealId = mealId
      Meal = HasNotStartedYet }

let init mealId =
    defaultState mealId, Cmd.ofMsg (GetMeal Started)

let update msg state =
    match msg with
    | GetMeal Started ->
        let loadMeal =
            async {
                let! meal = Api.mealApi.GetMeal(state.MealId)
                return GetMeal(Finished meal)
            }

        { state with Meal = InProgress }, Cmd.fromAsync loadMeal
    | GetMeal (Finished meal) -> { state with Meal = (Resolved meal) }, Cmd.none
