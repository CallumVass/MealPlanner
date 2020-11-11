module CategoryList.Types

open System
open Shared.Types

type State =
    { Categories: Deferred<MealCategory list> }

type Msg =
    | GetCategories of AsyncOperationStatus<MealCategory list>
    | DeleteCategory of Guid
