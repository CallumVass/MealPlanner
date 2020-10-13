module EditMeal.App

open Elmish
open EditMeal.Types

let defaultState mealId = { MealId = mealId }

let init mealId = defaultState mealId, Cmd.none

let update msg state = state, Cmd.none
