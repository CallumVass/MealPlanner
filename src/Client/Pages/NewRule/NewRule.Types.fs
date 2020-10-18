module NewRule.Types

open Shared.Types
open Form

type State = { Rule: ValidatedForm<Rule> }

type Msg =
    | FormChanged of Rule
    | Save
