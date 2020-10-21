module Home.App

open Elmish
open Home.Types
open Shared.Types
open Api
open Home.Logic
open Form.Types
open Shared.Home.Validation

let defaultOptions =
    { DaysBetweenSameMeal = 14
      DaysToCalculate = 7 }

let defaultState =
    { Options = defaultOptions |> ValidatedForm.init
      AvailableMeals = HasNotStartedYet
      ChosenMeals = [] }

let init () =
    defaultState, Cmd.ofMsg (GetMeals Started)

let update msg state =
    match msg with
    | GetMeals Started ->
        let loadMeals =
            async {
                let! meals = mealApi.GetMeals()
                return GetMeals(Finished meals)
            }

        { state with
              AvailableMeals = InProgress },
        Cmd.fromAsync loadMeals
    | GetMeals (Finished meals) ->
        { state with
              AvailableMeals = Resolved meals },
        Cmd.none
    | Calculate -> state |> calculate, Cmd.none
    | FormChanged f ->
        let newState =
            match state.AvailableMeals with
            | Resolved meals ->
                { state with
                      Options =
                          state.Options
                          |> ValidatedForm.updateWith f
                          |> ValidatedForm.validateWith (validateOptions meals.Length) }
            | _ ->
                { state with
                      Options =
                          state.Options
                          |> ValidatedForm.updateWith f
                          |> ValidatedForm.validateWith (validateOptions 1) }

        newState, Cmd.none
