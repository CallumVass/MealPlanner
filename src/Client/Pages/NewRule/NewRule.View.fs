module NewRule.View

open Elmish
open Feliz
open Feliz.UseElmish
open NewRule.App
open NewRule.Types

let private renderMainBody dispatch state =
    match state.DaysOfWeek with
    | Resolved daysOfWeek ->
        state.Rule
        |> Rule.render dispatch daysOfWeek FormChanged TrySave
    | _ -> Html.none

let private view =
    fun () ->
        let state, dispatch = React.useElmish (init, update, [||])

        let children = [ (state |> renderMainBody dispatch) ]

        children |> View.renderBody

let render =
    React.functionComponent ("EditMeal", view)
