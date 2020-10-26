module NewMeal.App

open System
open Elmish
open NewMeal.Types
open Form.Types
open Shared.Meal.Validation
open Feliz.Router
open Shared.Types

let defaultMeal =
    { Id = Guid.Empty
      Name = ""
      Category = None
      Rules = [] }

let defaultState =
    { Meal = defaultMeal |> ValidatedForm.init
      Categories = HasNotStartedYet
      Rules = HasNotStartedYet }

let init =

    let messages =
        [ Cmd.ofMsg (GetRules Started)
          Cmd.ofMsg (GetCategories Started) ]

    defaultState, Cmd.batch messages

let update msg (state: State) =
    match msg with
    | GetRules Started ->
        let loadRules =
            async {
                let! rules = Api.mealApi.GetRules()
                return GetRules(Finished rules)
            }

        { state with Rules = InProgress }, Cmd.fromAsync loadRules
    | GetRules (Finished rules) -> { state with Rules = Resolved rules }, Cmd.none
    | GetCategories Started ->
        let loadCategories =
            async {
                let! categories = Api.mealApi.GetCategories()
                return GetCategories(Finished categories)
            }

        { state with Categories = InProgress }, Cmd.fromAsync loadCategories
    | GetCategories (Finished categories) ->
        { state with
              Categories = Resolved categories },
        Cmd.none
    | FormChanged f ->
        { state with
              Meal =
                  state.Meal
                  |> ValidatedForm.updateWith f
                  |> ValidatedForm.validateWith validateMeal },
        Cmd.none
    | TrySave meal ->
        let validatedMeal =
            meal |> ValidatedForm.validateWith validateMeal

        let newState = { state with Meal = validatedMeal }
        if validatedMeal.ValidationErrors.IsEmpty then newState, Cmd.ofMsg Save else newState, Cmd.none
    | FormSaved -> state, Cmd.navigate ("")
    | Save ->
        let saveForm =
            async {
                let! _ = Api.mealApi.AddMeal state.Meal.FormData
                return FormSaved
            }

        state, Cmd.fromAsync saveForm
