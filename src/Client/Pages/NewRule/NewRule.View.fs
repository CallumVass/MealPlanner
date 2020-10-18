module NewRule.View

open Elmish
open Feliz
open Feliz.UseElmish
open NewRule.App

let private view =
    fun () ->
        let state, dispatch = React.useElmish (init, update, [||])

        let children = []

        children |> ViewHelpers.renderBody

let render =
    React.functionComponent ("EditMeal", view)
