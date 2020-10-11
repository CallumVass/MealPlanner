module Main.View

open Feliz
open Feliz.Router
open Types
open Main.Logic

let renderHeader =
    let h1 =
        Html.h1 [ prop.className "text-3xl font-semibold text-white"
                  prop.text "Meal Planner" ]

    Html.div [ prop.className "bg-gradient-to-br from-purple-400 to-purple-700 flex p-4 mb-6 justify-between"
               prop.children h1 ]

let renderLoginButton =
    Html.div [ prop.className "w-full"
               prop.children (ViewHelpers.buttonLink "Login with Google" "/login") ]

let renderCurrentState activePage state =
    match state.User with
    | Authenticated -> activePage
    | Anonymous -> renderLoginButton

let render (state: State) (dispatch: Msg -> unit) =
    let activePage =
        match state.CurrentPage with
        | Page.Index index -> Index.View.render index (IndexMsg >> dispatch)

    let body = (state |> renderCurrentState activePage)

    let page =
        Html.div [ prop.className "min-h-screen bg-gray-200 text-gray-800"
                   prop.children [ renderHeader; body ] ]

    React.router [ router.onUrlChanged (parseUrl >> UrlChanged >> dispatch)
                   router.children page ]
