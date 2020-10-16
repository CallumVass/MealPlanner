module EditMeal.Types

open System
open Shared.Types
open Form

type State =
    { MealId: Guid
      Meal: Deferred<ValidatedForm<Meal> option> }

type Msg =
    | GetMeal of AsyncOperationStatus<Meal option>
    | FormChanged of Meal
    | Save of Meal
