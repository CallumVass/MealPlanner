module Index

open Elmish
open Fable.Remoting.Client
open Index.Domain
open Shared
open System

let mealApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IMealApi>

type Msg =
    | GotMeals of Meal list
    | Calculate
    | AddMeal
    | ChangeDaysBetweenSameMeal of string
    | ChangeDaysToCalculate of string

let defaultOptions =
    { DaysBetweenSameMeal = 4
      DaysToCalculate = 7 }

let init () =
    { Options = defaultOptions
      AvailableMeals = []
      ChosenMeals = [] },
    Cmd.OfAsync.perform mealApi.GetMeals () GotMeals

let update msg model =
    match msg with
    | GotMeals meals -> { model with AvailableMeals = meals }, Cmd.none
    | AddMeal -> model, Cmd.none
    | Calculate -> (calculate model), Cmd.none
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
