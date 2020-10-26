module Home.App

open System
open Elmish
open Home.Types
open Shared.Types
open Api
open Home.Logic
open Form.Types
open Shared.Home.Validation

let defaultOptions =
    { DaysBetweenSameMeal = 14
      DaysToCalculate = 1
      DaysBetweenSameMealCategory = 0
      FromDate = DateTime.Now.AddDays(float 1).Date }

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
        let formData =
            { state.Options.FormData with
                  DaysToCalculate = Math.Min(meals.Length, 7) }

        let options =
            { state.Options with
                  FormData = formData }

        { state with
              AvailableMeals = Resolved meals
              Options = options },
        Cmd.none
    | Calculate -> state |> calculate, Cmd.none
    | FormChanged f ->
        let newState =
            { state with
                  Options =
                      state.Options
                      |> ValidatedForm.updateWith f
                      |> ValidatedForm.validateWith validateOptions }

        newState, Cmd.none
