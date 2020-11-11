module RuleList.Types

open System
open Shared.Types

type State = { Rules: Deferred<Rule list> }

type Msg =
    | GetRules of AsyncOperationStatus<Rule list>
    | DeleteRule of Guid
