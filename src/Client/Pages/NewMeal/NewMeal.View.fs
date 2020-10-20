module NewMeal.View

open Feliz
open NewMeal.App
open NewMeal.Types
open Feliz.UseElmish

let private renderMainBody dispatch state =
    match state.Rules with
    | Resolved rules ->
        state.Meal
        |> Meal.render dispatch rules FormChanged TrySave
    | _ -> Html.none

let private view =
    fun () ->
        let state, dispatch = React.useElmish (init, update, [||])

        let children = [ (state |> renderMainBody dispatch) ]

        children |> View.renderBody

let render =
    React.functionComponent ("NewMeal", view)
