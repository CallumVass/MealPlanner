module RuleList.App

open Api
open Elmish
open RuleList.Types
open Shared.Types

let defaultState = { Rules = HasNotStartedYet }

let init () =
    defaultState, Cmd.ofMsg (GetRules Started)

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | DeleteRule ruleId ->
        let deleteRule =
            async {
                let! _ = mealApi.DeleteRule ruleId
                return GetRules Started
            }

        state, Cmd.fromAsync deleteRule
    | GetRules Started ->
        let loadRules =
            async {
                let! rules = mealApi.GetRules()
                return GetRules(Finished rules)
            }

        { state with Rules = InProgress }, Cmd.fromAsync loadRules
    | GetRules (Finished rules) -> { state with Rules = Resolved rules }, Cmd.none
