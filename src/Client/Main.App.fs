module Main.App

open Main.Logic
open Main.Types
open Elmish

open Feliz
open Feliz.Router

let init () =
    let defaultState =
        { User = Anonymous
          CurrentUrl = Router.currentUrl () }

    defaultState, performAuthCheck (Cmd.none)

let update (msg: Msg) (state: State) =
    match msg with
    | AuthCheck (isAuth, nextCmd) -> state |> (checkIsAuth isAuth nextCmd)
    | UrlChanged segments -> { state with CurrentUrl = segments }, Cmd.none
