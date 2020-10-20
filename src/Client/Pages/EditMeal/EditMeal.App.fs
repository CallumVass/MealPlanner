module EditMeal.App

open Elmish
open EditMeal.Types
open Form.Types
open Shared.Meal.Validation
open Feliz.Router

let defaultState mealId =
    { MealId = mealId
      Meal = HasNotStartedYet
      Rules = HasNotStartedYet }

let init mealId =

    let messages =
        [ Cmd.ofMsg (GetMeal Started)
          Cmd.ofMsg (GetRules Started) ]

    defaultState mealId, Cmd.batch messages

let private resolveForm fn state f =
    let form = f |> fn
    { state with
          Meal = Resolved(Some form) }

let private updateResolvedState model fn state =
    let updatedState =
        model
        |> Option.map (resolveForm fn state)
        |> Option.defaultValue { state with Meal = Resolved None }

    updatedState, Cmd.none

let update msg state =
    match msg with
    | GetRules Started ->
        let loadRules =
            async {
                let! rules = Api.mealApi.GetRules()
                return GetRules(Finished rules)
            }

        { state with Rules = InProgress }, Cmd.fromAsync loadRules
    | GetRules (Finished rules) -> { state with Rules = Resolved rules }, Cmd.none
    | GetMeal Started ->
        let loadMeal =
            async {
                let! meal = Api.mealApi.GetMeal(state.MealId)
                return GetMeal(Finished meal)
            }

        { state with Meal = InProgress }, Cmd.fromAsync loadMeal
    | GetMeal (Finished meal) ->
        state
        |> updateResolvedState meal ValidatedForm.init
    | FormChanged f ->
        match state.Meal with
        | Resolved m ->
            state
            |> updateResolvedState m (f |> ValidatedForm.updateWith)
        | _ -> state, Cmd.none
    | TrySave meal ->
        let validatedMeal =
            meal |> ValidatedForm.validateWith validateMeal

        let newState =
            { state with
                  Meal = Resolved(Some validatedMeal) }

        if validatedMeal.ValidationErrors.IsEmpty then newState, Cmd.ofMsg (Save validatedMeal) else newState, Cmd.none
    | FormSaved -> state, Cmd.navigate ("")
    | Save meal ->
        let saveForm =
            async {
                let! _ = Api.mealApi.EditMeal meal.FormData
                return FormSaved
            }

        state, Cmd.fromAsync saveForm
