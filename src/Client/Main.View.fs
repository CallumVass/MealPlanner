module Main.View

open Feliz
open Feliz.Router
open Types
open Main.Logic

let linkContainer (link: ReactElement) =
    Html.span [ prop.className "mr-3"
                prop.children link ]

let links =
    [ linkContainer (ViewHelpers.buttonLink "Add Meal" (Router.format ("meals", "new")))
      linkContainer (ViewHelpers.buttonLink "Add Rule" (Router.format ("rules", "new"))) ]

let renderLinks state =
    match state.User with
    | Anonymous -> Html.none
    | Authenticated -> Html.div links

let renderHeader state dispatch =
    let h1 =
        Html.h1 [ prop.className "text-3xl font-semibold text-white"
                  prop.text "Meal Planner" ]

    let links = renderLinks state

    Html.div [ prop.className
                   "bg-gradient-to-br from-purple-400 to-purple-700 flex p-4 mb-6 justify-between items-center"
               prop.children [ h1; links ] ]

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
        | Page.Index page -> Index.View.render page (IndexMsg >> dispatch)
        | Page.EditMeal page -> EditMeal.View.render page (EditMealMsg >> dispatch)

    let body = (state |> renderCurrentState activePage)

    let page =
        Html.div [ prop.className "min-h-screen bg-gray-200 text-gray-800"
                   prop.children [ renderHeader state dispatch
                                   body ] ]

    React.router [ router.onUrlChanged (parseUrl >> UrlChanged >> dispatch)
                   router.children page ]
