module NewRule.Types

open System
open Shared.Types
open Form.Types

type State =
    { Rule: ValidatedForm<Rule>
      DaysOfWeek: Deferred<DayOfWeek list> }

type Msg =
    | GetDaysOfWeek of AsyncOperationStatus<DayOfWeek list>
    | FormChanged of Rule
    | TrySave
    | Save
    | FormSaved
