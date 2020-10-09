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
    | MaybeGotMeals of Result<Meal list, string>
    | Calculate
    | AddMeal
    | ChangeDaysBetweenSameMeal of string
    | ChangeDaysToCalculate of string

let defaultOptions =
    { DaysBetweenSameMeal = 4
      DaysToCalculate = 7 }

let init () =
    { Options = defaultOptions
      UserData = Unauthenticated },
    Cmd.OfAsync.perform mealApi.GetMeals () MaybeGotMeals

let update msg model =
    match msg, model.UserData with
    | MaybeGotMeals maybeMeals, _ -> applyMeals model maybeMeals, Cmd.none
    | AddMeal, Authenticated user -> model, Cmd.none
    | AddMeal, _ -> model, Cmd.none
    | Calculate, Authenticated user ->
        { model with
              UserData = Authenticated(calculate user model.Options) },
        Cmd.none
    | Calculate, _ -> model, Cmd.none
    | ChangeDaysBetweenSameMeal newValue, _ ->
        let value =
            match String.IsNullOrEmpty newValue with
            | true -> None
            | false -> newValue |> int |> Some

        let newOptions =
            { model.Options with
                  DaysBetweenSameMeal = value |> Option.defaultValue 0 }

        { model with Options = newOptions }, Cmd.none
    | ChangeDaysToCalculate newValue, _ ->
        let value =
            match String.IsNullOrEmpty newValue with
            | true -> None
            | false -> newValue |> int |> Some

        let newOptions =
            { model.Options with
                  DaysToCalculate = value |> Option.defaultValue 0 }

        { model with Options = newOptions }, Cmd.none
