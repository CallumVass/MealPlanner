module NewCategory.View

open NewCategory.App
open NewCategory.Types
open Feliz
open Feliz.UseElmish

let private renderMainBody dispatch state =

    let inputs =
        [ Form.textInput "Name" (nameof state.Category.FormData.Name) state.Category.FormData.Name
              state.Category.ValidationErrors (fun x ->
              { state.Category.FormData with
                    Name = x }
              |> FormChanged
              |> dispatch) ]

    let form =
        Html.div [ prop.className "flex flex-wrap pb-2"
                   prop.children inputs ]

    let addCategoryButton =
        View.greenButton (not state.Category.ValidationErrors.IsEmpty) "Save" (fun _ -> Save |> dispatch)

    let formContainer =
        View.box [ (View.h2 "New Category")
                   form
                   addCategoryButton ]

    Html.div [ prop.className "w-full mb-2"
               prop.children formContainer ]

let private view =
    fun () ->
        let state, dispatch = React.useElmish (init, update, [||])

        let children = [ (state |> renderMainBody dispatch) ]

        children |> View.renderBody

let render =
    React.functionComponent ("NewCategory", view)
