module NewCategory.App

open System
open Elmish
open NewCategory.Types
open Form.Types
open Shared.Category.Validation
open Feliz.Router
open Shared.Types

let defaultCategory = { Id = Guid.Empty; Name = "" }

let defaultState =
    { Category = defaultCategory |> ValidatedForm.init }

let init = defaultState, Cmd.none

let update msg (state: State) =
    match msg with
    | FormChanged f ->
        { state with
              Category =
                  state.Category
                  |> ValidatedForm.updateWith f
                  |> ValidatedForm.validateWith validateCategory },
        Cmd.none
    | FormSaved -> state, Cmd.navigate ("")
    | Save ->
        let validatedForm =
            { state with
                  Category =
                      state.Category
                      |> ValidatedForm.validateWith validateCategory }

        if validatedForm.Category.ValidationErrors.IsEmpty then
            let saveForm =
                async {
                    let! _ = Api.mealApi.AddCategory state.Category.FormData
                    return FormSaved
                }

            state, Cmd.fromAsync saveForm
        else
            validatedForm, Cmd.none
