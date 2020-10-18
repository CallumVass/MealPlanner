module NewRule.App

open System
open Elmish
open NewRule.Types
open Shared.Types
open Form

let defaultRule =
    { Id = Guid.Empty
      Name = ""
      ApplicableOn = [] }

let defaultState =
    { Rule = defaultRule |> ValidatedForm.init }

let init = defaultState, Cmd.none

let update msg state =
    match msg with
    | FormChanged f ->
        { state with
              Rule = state.Rule |> ValidatedForm.updateWith f },
        Cmd.none
    | Save -> state, Cmd.none
