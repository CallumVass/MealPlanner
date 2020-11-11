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
        let saveForm =
            async {
                let! _ = Api.mealApi.AddCategory state.Category.FormData
                return FormSaved
            }

        let newCategory = { state.Category with IsLoading = true }

        { state with Category = newCategory }, Cmd.fromAsync saveForm
    | TrySave category ->

        let validatedCategory =
            category
            |> ValidatedForm.validateWith validateCategory

        let newState =
            { state with
                  Category = validatedCategory }

        if validatedCategory.ValidationErrors.IsEmpty
        then newState, Cmd.ofMsg Save
        else newState, Cmd.none
