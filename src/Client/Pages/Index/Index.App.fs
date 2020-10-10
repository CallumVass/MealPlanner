module Index.App

open Elmish
open Fable.Remoting.Client
open Index.State
open Shared
open System

let anonymousApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IAnonymousApi>

let mealApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IMealApi>

type Msg =
    | AuthCheck of bool
    | GotMeals of Meal list
    | Calculate
    | ChangeDaysBetweenSameMeal of string
    | ChangeDaysToCalculate of string

let defaultOptions =
    { DaysBetweenSameMeal = 4
      DaysToCalculate = 7 }

let defaultModel =
    { Options = defaultOptions
      AvailableMeals = []
      ChosenMeals = [] }

let init () =
    { UserData = Unauthenticated }, Cmd.OfAsync.perform anonymousApi.IsAuthenticated () AuthCheck

let authCheck model isAuthenticated =
    if isAuthenticated then
        { model with
              UserData = Authenticated defaultModel },
        Cmd.OfAsync.perform mealApi.GetMeals () GotMeals
    else
        model, Cmd.none

let update msg model =
    match msg, model.UserData with
    | AuthCheck isAuthenticated, _ -> isAuthenticated |> authCheck model
    | GotMeals meals, Authenticated user ->
        let newUser = { user with AvailableMeals = meals }
        { model with
              UserData = Authenticated newUser },
        Cmd.none
    | Calculate, Authenticated user ->
        { model with
              UserData = Authenticated(calculate user) },
        Cmd.none
    | Calculate, _ -> model, Cmd.none
    | ChangeDaysBetweenSameMeal newValue, Authenticated user ->
        let value =
            match String.IsNullOrEmpty newValue with
            | true -> None
            | false -> newValue |> int |> Some

        let newOptions =
            { user.Options with
                  DaysBetweenSameMeal = value |> Option.defaultValue 0 }

        let newUserData = { user with Options = newOptions }

        { model with
              UserData = Authenticated newUserData },
        Cmd.none
    | ChangeDaysToCalculate newValue, Authenticated user ->
        let value =
            match String.IsNullOrEmpty newValue with
            | true -> None
            | false -> newValue |> int |> Some

        let newOptions =
            { user.Options with
                  DaysToCalculate = value |> Option.defaultValue 0 }

        let newUserData = { user with Options = newOptions }

        { model with
              UserData = Authenticated newUserData },
        Cmd.none
    | _, Unauthenticated -> model, Cmd.none
