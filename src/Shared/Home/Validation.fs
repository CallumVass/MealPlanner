module Shared.Home.Validation

open Shared.Validation
open Shared.Types

let validateOptions (o: MealOptions) =
    [ (nameof o.DaysBetweenSameMeal), (validateRange 0 14 o.DaysBetweenSameMeal)
      (nameof o.DaysToCalculate), (validateRange 1 30 o.DaysToCalculate) ]
    |> validate
