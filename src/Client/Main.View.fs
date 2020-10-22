module Main.View

open Feliz
open Feliz.Router
open Types

let private linkContainer (link: ReactElement) =
    Html.div [ prop.className "mr-3"
               prop.children link ]

let private links =
    [ linkContainer (View.buttonLink "Add Meal" (Router.format ("meals", "new")))
      linkContainer (View.buttonLink "Add Rule" (Router.format ("rules", "new"))) ]

let private renderLinks state =
    match state.User with
    | Anonymous -> Html.none
    | Authenticated ->
        Html.div [ prop.className "flex flex-wrap mt-2 md:mt-0"
                   prop.children links ]

let private renderHeader state =
    let h1 =
        Html.h1 [ prop.className "text-3xl font-semibold text-white"
                  prop.children
                      [ Html.a [ prop.text "Meal Planner"
                                 prop.href (Router.format ("")) ] ] ]

    let links = renderLinks state

    Html.div [ prop.className
                   "bg-gradient-to-br from-purple-400 to-purple-700 flex p-4 mb-4 justify-between items-center flex-col md:flex-row"
               prop.children [ h1; links ] ]

let private renderLoginButton =
    Html.div [ prop.className "w-full"
               prop.children
                   [ Html.div [ prop.className "mx-4"
                                prop.children (View.buttonLink "Login with Google" "/login") ] ] ]

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
        | [ "meals"; "new" ] -> NewMeal.View.render ()
        | _ -> Home.View.render ()

    let body =
        state |> (renderCurrentState (activePage))

    let page =
        Html.div [ prop.className "min-h-screen bg-gray-200 text-gray-800"
                   prop.children [ renderHeader state
                                   body ] ]

    React.router [ router.onUrlChanged (UrlChanged >> dispatch)
                   router.children page ]
