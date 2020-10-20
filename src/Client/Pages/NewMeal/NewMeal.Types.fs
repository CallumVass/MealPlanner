module NewMeal.Types

open Shared.Types
open Form.Types

type State =
    { Rules: Deferred<Rule list>
      Meal: ValidatedForm<Meal> }

type Msg =
    | GetRules of AsyncOperationStatus<Rule list>
    | FormChanged of Meal
    | TrySave of ValidatedForm<Meal>
    | Save
    | FormSaved
