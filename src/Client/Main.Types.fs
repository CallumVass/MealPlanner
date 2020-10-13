module Main.Types

open Elmish
open System

type ApplicationUser =
    | Anonymous
    | Authenticated

type State =
    { User: ApplicationUser
      CurrentUrl: string list }

type Msg =
    | AuthCheck of bool * Cmd<Msg>
    | UrlChanged of string list
