module EditMeal.Types

open System
open Shared.Types
open Form

type State =
    { MealId: Guid
      Meal: Deferred<Meal option> }

type Msg = GetMeal of AsyncOperationStatus<Meal option>
