module EditRule.Types

open System
open Shared.Types
open Form.Types

type State =
    { RuleId: Guid
      DaysOfWeek: Deferred<DayOfWeek list>
      Rule: Deferred<ValidatedForm<Rule> option> }

type Msg =
    | GetRule of AsyncOperationStatus<Rule option>
    | GetDaysOfWeek of AsyncOperationStatus<DayOfWeek list>
    | FormChanged of Rule
    | TrySave of ValidatedForm<Rule>
    | Save of ValidatedForm<Rule>
    | FormSaved
