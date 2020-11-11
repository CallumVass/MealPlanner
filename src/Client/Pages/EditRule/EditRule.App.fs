module EditRule.App

open Elmish
open EditRule.Types
open Form.Types
open Shared.Rule.Validation
open Feliz.Router

let defaultState ruleId =
    { RuleId = ruleId
      Rule = HasNotStartedYet
      DaysOfWeek = HasNotStartedYet }

let init ruleId =

    let messages =
        [ Cmd.ofMsg (GetRule Started)
          Cmd.ofMsg (GetDaysOfWeek Started) ]

    defaultState ruleId, Cmd.batch messages

let private resolveForm fn state f =
    let form = f |> fn |> ValidatedForm.validateWith validateRule
    { state with
          Rule = Resolved(Some form) }

let private updateResolvedState model fn state =
    let updatedState =
        model
        |> Option.map (resolveForm fn state)
        |> Option.defaultValue { state with Rule = Resolved None }

    updatedState, Cmd.none

let update msg state =
    match msg with
    | GetDaysOfWeek Started ->
        let loadDaysOfWeek =
            async {
                let! daysOfWeek = Api.mealApi.GetDaysOfWeek()
                return GetDaysOfWeek(Finished daysOfWeek)
            }

        { state with DaysOfWeek = InProgress }, Cmd.fromAsync loadDaysOfWeek
    | GetDaysOfWeek (Finished daysOfWeek) ->
        { state with
              DaysOfWeek = Resolved daysOfWeek },
        Cmd.none
    | GetRule Started ->
        let loadRule =
            async {
                let! rule = Api.mealApi.GetRule(state.RuleId)
                return GetRule(Finished rule)
            }

        { state with Rule = InProgress }, Cmd.fromAsync loadRule
    | GetRule (Finished rule) ->
        state
        |> updateResolvedState rule ValidatedForm.init
    | FormChanged f ->
        match state.Rule with
        | Resolved m ->
            state
            |> updateResolvedState m (f |> ValidatedForm.updateWith)
        | _ -> state, Cmd.none
    | TrySave rule ->
        let validatedRule =
            rule |> ValidatedForm.validateWith validateRule

        let newState =
            { state with
                  Rule = Resolved(Some validatedRule) }

        if validatedRule.ValidationErrors.IsEmpty then newState, Cmd.ofMsg (Save validatedRule) else newState, Cmd.none
    | FormSaved -> state, Cmd.navigate ("")
    | Save rule ->
        let saveForm =
            async {
                let! _ = Api.mealApi.EditRule rule.FormData
                return FormSaved
            }

        state, Cmd.fromAsync saveForm
