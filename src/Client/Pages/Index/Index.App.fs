module Index.App

open Elmish
open Index.Types
open Shared
open System
open Api
open Index.Logic

let defaultOptions =
    { DaysBetweenSameMeal = 4
      DaysToCalculate = 7 }

let defaultState =
    { Options = defaultOptions
      AvailableMeals = []
      ChosenMeals = [] }

let init () =
    defaultState, Cmd.OfAsync.perform mealApi.GetMeals () GotMeals

let update msg state =
    match msg with
    | GotMeals meals -> { state with AvailableMeals = meals }, Cmd.none
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
