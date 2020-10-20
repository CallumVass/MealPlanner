module Shared.Meal.Validation

open Shared.Validation
open Shared.Types

let validateMeal (meal: Meal) =
    [ (nameof meal.Name), (validateNotEmpty meal.Name) ]
    |> validate
