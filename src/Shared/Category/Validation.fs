module Shared.Category.Validation

open Shared
open Validation
open Shared.Types

let validateCategory (category: MealCategory) =
    [ (nameof category.Name), (validateNotEmpty category.Name) ]
    |> validate
