module Main.Types

open Elmish

type ApplicationUser =
    | Anonymous
    | Authenticated

[<RequireQualifiedAccess>]
type Url =
    | Index
    // | EditMeal of int
    | Unknown

[<RequireQualifiedAccess>]
type Page = Index of Index.Types.State
// | EditMeal of EditMeal.State

type State =
    { User: ApplicationUser
      CurrentUrl: Url
      CurrentPage: Page }

type Msg =
    | AuthCheck of bool * Cmd<Msg>
    | IndexMsg of Index.Types.Msg
    // | EditMealMsg of EditMeal.App.Msg
    | UrlChanged of Url
