module EditCategory.Types

open System
open Shared.Types
open Form.Types

type State =
    { CategoryId: Guid
      Category: Deferred<ValidatedForm<MealCategory> option> }

type Msg =
    | GetCategory of AsyncOperationStatus<MealCategory option>
    | FormChanged of MealCategory
    | TrySave of ValidatedForm<MealCategory>
    | Save of ValidatedForm<MealCategory>
    | FormSaved
