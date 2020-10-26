module NewCategory.Types

open Shared.Types
open Form.Types

type State =
    { Category: ValidatedForm<MealCategory> }


type Msg =
    | FormChanged of MealCategory
    | Save
    | FormSaved
