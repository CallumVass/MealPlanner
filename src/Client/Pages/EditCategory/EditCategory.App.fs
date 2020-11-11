module EditCategory.App

open Elmish
open EditCategory.Types
open Form.Types
open Shared.Category.Validation
open Feliz.Router

let defaultState categoryId =
    { CategoryId = categoryId
      Category = HasNotStartedYet }

let init categoryId =
    defaultState categoryId, Cmd.ofMsg (GetCategory Started)

let private resolveForm fn state f =
    let form =
        f
        |> fn
        |> ValidatedForm.validateWith validateCategory

    { state with
          Category = Resolved(Some form) }

let private updateResolvedState model fn state =
    let updatedState =
        model
        |> Option.map (resolveForm fn state)
        |> Option.defaultValue { state with Category = Resolved None }

    updatedState, Cmd.none

let update msg state =
    match msg with
    | GetCategory Started ->
        let loadCategory =
            async {
                let! category = Api.mealApi.GetCategory(state.CategoryId)
                return GetCategory(Finished category)
            }

        { state with Category = InProgress }, Cmd.fromAsync loadCategory
    | GetCategory (Finished category) ->
        state
        |> updateResolvedState category ValidatedForm.init
    | FormChanged f ->
        match state.Category with
        | Resolved m ->
            state
            |> updateResolvedState m (f |> ValidatedForm.updateWith)
        | _ -> state, Cmd.none
    | TrySave category ->
        let validatedCategory =
            category
            |> ValidatedForm.validateWith validateCategory

        let newState =
            { state with
                  Category = Resolved(Some validatedCategory) }

        if validatedCategory.ValidationErrors.IsEmpty
        then newState, Cmd.ofMsg (Save validatedCategory)
        else newState, Cmd.none
    | FormSaved -> state, Cmd.navigate ("")
    | Save category ->
        let saveForm =
            async {
                let! _ = Api.mealApi.EditCategory category.FormData
                return FormSaved
            }

        state, Cmd.fromAsync saveForm
