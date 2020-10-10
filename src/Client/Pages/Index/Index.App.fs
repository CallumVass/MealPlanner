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

let defaultModel =
    { Options = defaultOptions
      AvailableMeals = []
      ChosenMeals = [] }

let init () =
    defaultModel, Cmd.OfAsync.perform mealApi.GetMeals () GotMeals


let update msg model =
    match msg with
    | GotMeals meals -> { model with AvailableMeals = meals }, Cmd.none
    | Calculate -> model |> calculate, Cmd.none
    | ChangeDaysBetweenSameMeal newValue ->
        let value =
            match String.IsNullOrEmpty newValue with
            | true -> None
            | false -> newValue |> int |> Some

        let newOptions =
            { model.Options with
                  DaysBetweenSameMeal = value |> Option.defaultValue 0 }

        { model with Options = newOptions }, Cmd.none
    | ChangeDaysToCalculate newValue ->
        let value =
            match String.IsNullOrEmpty newValue with
            | true -> None
            | false -> newValue |> int |> Some

        let newOptions =
            { model.Options with
                  DaysToCalculate = value |> Option.defaultValue 0 }

        { model with Options = newOptions }, Cmd.none
