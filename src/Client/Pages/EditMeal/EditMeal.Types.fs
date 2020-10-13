module EditMeal.Types

open Shared

type State = { Id: string }

type Msg =
    | GotMeals of Meal list
    | Calculate
    | ChangeDaysBetweenSameMeal of string
    | ChangeDaysToCalculate of string
