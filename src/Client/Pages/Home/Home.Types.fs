module Home.Types

open Shared.Types
open Form

type State =
    { Options: ValidatedForm<MealOptions>
      AvailableMeals: Deferred<Meal list>
      ChosenMeals: (string * string * Meal) list }

type Msg =
    | GetMeals of AsyncOperationStatus<Meal list>
    | Calculate
    | FormChanged of MealOptions
