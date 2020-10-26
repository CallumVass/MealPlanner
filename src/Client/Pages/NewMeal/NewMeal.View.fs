module NewMeal.View

open Feliz
open NewMeal.App
open NewMeal.Types
open Feliz.UseElmish

let private renderMainBody dispatch state =
    match state.Rules, state.Categories with
    | Resolved rules, Resolved categories ->
        state.Meal
        |> Meal.render dispatch rules categories FormChanged TrySave
    | _, _ -> Html.none

let private view =
    fun () ->
        let state, dispatch = React.useElmish (init, update, [||])

        let children = [ (state |> renderMainBody dispatch) ]

        children |> View.renderBody

let render =
    React.functionComponent ("NewMeal", view)
