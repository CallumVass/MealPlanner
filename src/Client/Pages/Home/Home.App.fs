module Home.App

open Elmish
open Home.Types
open Shared.Types
open System
open Api
open Home.Logic

let defaultOptions =
    { DaysBetweenSameMeal = 14
      DaysToCalculate = 7 }

let defaultState =
    { Options = defaultOptions
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
    | ChangeDaysBetweenSameMeal newValue ->
        let value =
            match String.IsNullOrEmpty newValue with
            | true -> None
            | false -> newValue |> int |> Some

        let newOptions =
            { state.Options with
                  DaysBetweenSameMeal = value |> Option.defaultValue 0 }

        { state with Options = newOptions }, Cmd.none
    | ChangeDaysToCalculate newValue ->
        let value =
            match String.IsNullOrEmpty newValue with
            | true -> None
            | false -> newValue |> int |> Some

        let newOptions =
            { state.Options with
                  DaysToCalculate = value |> Option.defaultValue 0 }

        { state with Options = newOptions }, Cmd.none
