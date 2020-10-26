module NewMeal.Types

open Shared.Types
open Form.Types

type State =
    { Rules: Deferred<Rule list>
      Categories: Deferred<MealCategory list>
      Meal: ValidatedForm<Meal> }

type Msg =
    | GetRules of AsyncOperationStatus<Rule list>
    | GetCategories of AsyncOperationStatus<MealCategory list>
    | FormChanged of Meal
    | TrySave of ValidatedForm<Meal>
    | Save
    | FormSaved
