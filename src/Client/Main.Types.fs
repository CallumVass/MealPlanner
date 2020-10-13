module Main.Types

open Elmish
open System

type ApplicationUser =
    | Anonymous
    | Authenticated

[<RequireQualifiedAccess>]
type Url =
    | Index
    | EditMeal of Guid
    | Unknown

[<RequireQualifiedAccess>]
type Page =
    | Index of Index.Types.State
    | EditMeal of EditMeal.Types.State

type State =
    { User: ApplicationUser
      CurrentUrl: Url
      CurrentPage: Page }

type Msg =
    | AuthCheck of bool * Cmd<Msg>
    | IndexMsg of Index.Types.Msg
    | EditMealMsg of EditMeal.Types.Msg
    | UrlChanged of Url
