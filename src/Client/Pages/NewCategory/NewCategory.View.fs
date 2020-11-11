module NewCategory.View

open NewCategory.App
open NewCategory.Types
open Feliz
open Feliz.UseElmish

let private view =
    fun () ->
        let state, dispatch = React.useElmish (init, update, [||])

        let children =
            [ (state.Category
               |> Category.render dispatch FormChanged TrySave) ]

        children |> View.renderBody

let render =
    React.functionComponent ("NewCategory", view)
