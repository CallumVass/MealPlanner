module Index.Types

open Shared

type State =
    { Options: MealOptions
      AvailableMeals: Meal list
      ChosenMeals: (string * string * Meal) list }

type Msg =
    | GotMeals of Meal list
    | Calculate
    | ChangeDaysBetweenSameMeal of string
    | ChangeDaysToCalculate of string
