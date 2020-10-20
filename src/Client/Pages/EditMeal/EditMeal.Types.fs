module EditMeal.Types

open System
open Shared.Types
open Form.Types

type State =
    { MealId: Guid
      Rules: Deferred<Rule list>
      Meal: Deferred<ValidatedForm<Meal> option> }

type Msg =
    | GetMeal of AsyncOperationStatus<Meal option>
    | GetRules of AsyncOperationStatus<Rule list>
    | FormChanged of Meal
    | TrySave of ValidatedForm<Meal>
    | Save of ValidatedForm<Meal>
    | FormSaved
