module Shared.Home.Validation

open Shared.Validation
open Shared.Types

let validateOptions mealCount (o: MealOptions) =
    [ (nameof o.DaysBetweenSameMeal), (validateRange 0 mealCount o.DaysBetweenSameMeal)
      (nameof o.DaysToCalculate), (validateRange 1 mealCount o.DaysToCalculate) ]
    |> validate
