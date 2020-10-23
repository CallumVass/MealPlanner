module Shared.Home.Validation

open System
open Shared.Validation
open Shared.Types

let validateOptions (o: MealOptions) =
    [ (nameof o.DaysBetweenSameMeal), (validateMinimum 0 o.DaysBetweenSameMeal)
      (nameof o.DaysToCalculate), (validateMinimum 1 o.DaysToCalculate)
      (nameof o.FromDate), (validateDateNotBefore DateTime.Now o.FromDate) ]
    |> validate
