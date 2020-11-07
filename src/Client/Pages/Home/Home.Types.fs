module Home.Types

open System
open Shared.Types
open Form.Types

type State =
    { Options: ValidatedForm<MealOptions>
      AvailableMeals: Deferred<Meal list>
      ChosenMeals: (string * string * Meal option) list }

type Msg =
    | GetMeals of AsyncOperationStatus<Meal list>
    | Calculate
    | DeleteMeal of Guid
    | FormChanged of MealOptions
