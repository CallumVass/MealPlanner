module Main.App

open Main.Logic
open Main.Types
open Elmish

open Feliz
open Feliz.Router

let init () =
    let initialUrl = parseUrl (Router.currentUrl ())

    let indexState, indexCmd = Index.App.init ()

    let defaultState =
        { User = Anonymous
          CurrentUrl = initialUrl
          CurrentPage = indexState |> Page.Index }

    let (state, cmd) =
        match initialUrl with
        | Url.Index ->
            let nextPage = Page.Index indexState
            { defaultState with
                  CurrentPage = nextPage },
            Cmd.map IndexMsg indexCmd

        | Url.EditMeal mealId ->
            let editMealState, editMealCmd = EditMeal.App.init mealId
            let nextPage = Page.EditMeal editMealState
            { defaultState with
                  CurrentPage = nextPage },
            Cmd.map EditMealMsg editMealCmd
        | Url.Unknown -> defaultState, Cmd.none

    state, performAuthCheck (cmd)

let getNextPageState update page msg state =
    let newPageState, nextCmd = update
    { state with
          CurrentPage = page newPageState },
    Cmd.map msg nextCmd

let update (msg: Msg) (state: State) =
    match msg, state.CurrentPage with
    | IndexMsg newMsg, Page.Index newState ->
        state
        |> getNextPageState (Index.App.update newMsg newState) Page.Index IndexMsg
    | EditMealMsg newMsg, Page.EditMeal newState ->
        state
        |> getNextPageState (EditMeal.App.update newMsg newState) Page.EditMeal EditMealMsg
    | AuthCheck (isAuth, nextCmd), _ -> state |> (checkIsAuth isAuth nextCmd)
    | UrlChanged nextUrl, _ ->
        let show page =
            { state with
                  CurrentPage = page
                  CurrentUrl = nextUrl }

        match nextUrl with
        | Url.Index ->
            let indexState, indexCmd = Index.App.init ()
            show (Page.Index indexState), Cmd.map IndexMsg indexCmd
        | Url.EditMeal mealId ->
            let editMealState, editMealCmd = EditMeal.App.init mealId
            show (Page.EditMeal editMealState), Cmd.map EditMealMsg editMealCmd
        | Url.Unknown -> state, Cmd.navigate "/"
    | _, _ -> state, Cmd.none
