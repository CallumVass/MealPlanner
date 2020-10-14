module Home.Types

open Shared.Types

type State =
    { Options: MealOptions
      AvailableMeals: Deferred<Meal list>
      ChosenMeals: (string * string * Meal) list }

type Msg =
    | GetMeals of AsyncOperationStatus<Meal list>
    | Calculate
    | ChangeDaysBetweenSameMeal of string
    | ChangeDaysToCalculate of string
