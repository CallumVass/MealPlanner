module CategoryList.View

open Feliz
open Feliz.UseElmish
open Feliz.Router
open CategoryList.App
open CategoryList.Types
open Shared.Types

let private renderCategoryItem dispatch (category: MealCategory) =

    let actions =
        Html.div [ prop.className "flex"
                   prop.children [ View.buttonLink
                                       "Edit"
                                       (Router.format ("categories", (sprintf "%A" category.Id), "edit"))
                                   View.enabledRedButton "Delete" (fun _ -> DeleteCategory category.Id |> dispatch) ] ]

    Html.div [ prop.className "flex justify-between items-center"
               prop.children [ Html.text category.Name
                               actions ] ]

let private linkContainer (link: ReactElement) =
    Html.div [ prop.className "mr-4 mt-3 flex justify-end"
               prop.children link ]

let private renderCategoryList dispatch categories =

    match categories with
    | HasNotStartedYet -> Html.none
    | InProgress -> Html.none
    | Resolved rules ->
        let ruleList =
            Html.ul [ prop.className "flex flex-col text-sm"
                      prop.children
                          [ for rule in rules ->
                              Html.li [ prop.className "p-4 border-b"
                                        prop.children (rule |> renderCategoryItem dispatch) ] ] ]

        Html.div [ prop.className "w-full mb-2"
                   prop.children
                       (View.box [ View.h2 "Available Categories"
                                   linkContainer (View.buttonLink "Add Category" (Router.format ("categories", "new")))
                                   ruleList ]) ]

let private view =
    fun () ->
        let state, dispatch = React.useElmish (init, update, [||])

        let children =
            [ state.Categories |> renderCategoryList dispatch ]

        children |> View.renderBody

let render =
    React.functionComponent ("CategoryList", view)
