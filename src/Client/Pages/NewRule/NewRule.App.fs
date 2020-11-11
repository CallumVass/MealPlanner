module NewRule.App

open System
open Elmish
open NewRule.Types
open Shared.Types
open Shared.Rule.Validation
open Form.Types
open Feliz.Router

let defaultRule =
    { Id = Guid.Empty
      Name = ""
      ApplicableOn = [] }

let defaultState =
    { Rule = defaultRule |> ValidatedForm.init
      DaysOfWeek = HasNotStartedYet }

let init =
    defaultState, Cmd.ofMsg (GetDaysOfWeek Started)

let update msg state =
    match msg with
    | GetDaysOfWeek Started ->
        let loadDaysOfWeek =
            async {
                let! days = Api.mealApi.GetDaysOfWeek()
                return GetDaysOfWeek(Finished days)
            }

        { state with DaysOfWeek = InProgress }, Cmd.fromAsync loadDaysOfWeek
    | GetDaysOfWeek (Finished days) ->
        { state with
              DaysOfWeek = Resolved(days) },
        Cmd.none
    | FormChanged f ->
        { state with
              Rule =
                  state.Rule
                  |> ValidatedForm.updateWith f
                  |> ValidatedForm.validateWith validateRule },
        Cmd.none
    | FormSaved ->
        let newRule = { state.Rule with IsLoading = false }

        { state with Rule = newRule }, Cmd.navigate ("")
    | Save ->
        let saveForm =
            async {
                let! _ = Api.mealApi.AddRule state.Rule.FormData
                return FormSaved
            }

        let newRule = { state.Rule with IsLoading = true }

        { state with Rule = newRule }, Cmd.fromAsync saveForm
    | TrySave rule ->

        let validatedRule =
            rule |> ValidatedForm.validateWith validateRule

        let newState = { state with Rule = validatedRule }
        if validatedRule.ValidationErrors.IsEmpty then newState, Cmd.ofMsg Save else newState, Cmd.none
