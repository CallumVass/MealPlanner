module Main.View

open Feliz
open Feliz.Router
open Types

let private linkContainer (link: ReactElement) =
    Html.span [ prop.className "mr-3"
                prop.children link ]

let private links =
    [ linkContainer (ViewHelpers.buttonLink "Add Meal" (Router.format ("meals", "new")))
      linkContainer (ViewHelpers.buttonLink "Add Rule" (Router.format ("rules", "new"))) ]

let private renderLinks state =
    match state.User with
    | Anonymous -> Html.none
    | Authenticated -> Html.div links

let private renderHeader state =
    let h1 =
        Html.h1 [ prop.className "text-3xl font-semibold text-white"
                  prop.text "Meal Planner" ]

    let links = renderLinks state

    Html.div [ prop.className
                   "bg-gradient-to-br from-purple-400 to-purple-700 flex p-4 mb-4 justify-between items-center"
               prop.children [ h1; links ] ]

let private renderLoginButton =
    Html.div [ prop.className "w-full"
               prop.children
                   [ Html.div [ prop.className "ml-2"
                                prop.children (ViewHelpers.buttonLink "Login with Google" "/login") ] ] ]

let private renderCurrentState activePage state =
    match state.User with
    | Authenticated -> activePage
    | Anonymous -> renderLoginButton

let render (state: State) (dispatch: Msg -> unit) =
    let activePage =
        match state.CurrentUrl with
        | [] -> Home.View.render ()
        | [ "meals"; Route.Guid mealId; "edit" ] -> EditMeal.View.render ({ MealId = mealId })
        | [ "rules"; "new" ] -> NewRule.View.render ()
        | _ -> Home.View.render ()

    let body =
        state |> (renderCurrentState (activePage))

    let page =
        Html.div [ prop.className "min-h-screen bg-gray-200 text-gray-800"
                   prop.children [ renderHeader state
                                   body ] ]

    React.router [ router.onUrlChanged (UrlChanged >> dispatch)
                   router.children page ]
