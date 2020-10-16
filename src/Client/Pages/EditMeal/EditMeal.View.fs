module EditMeal.View

open Elmish
open Feliz
open Feliz.UseElmish
open System
open EditMeal.App
open EditMeal.Types
open Form
open Shared.Types

type EditMealProps = { MealId: Guid }

let private renderMeal dispatch (meal: ValidatedForm<Meal> option) =

    match meal with
    | Some m ->

        let form =
            Html.div [ prop.className "flex pb-2"
                       prop.children
                           [ ViewHelpers.textInput "Name" (nameof m.FormData.Name) m.FormData.Name m.ValidationErrors (fun x ->
                                 { m.FormData with Name = x }
                                 |> FormChanged
                                 |> dispatch) ] ]

        let editMealButton =
            ViewHelpers.button (not m.ValidationErrors.IsEmpty) "Save" (fun _ -> (Save m.FormData) |> dispatch)

        Html.div [ prop.className "w-full px-2 mb-2"
                   prop.children
                       (ViewHelpers.box [ (ViewHelpers.h2 "Meals")
                                          form
                                          editMealButton ]) ]
    | None -> Html.none

let private renderMainBody state dispatch =
    match state.Meal with
    | Resolved meal -> meal |> renderMeal dispatch
    | _ -> Html.none

let private renderBody state dispatch =
    Html.div [ prop.className "px-2"
               prop.children
                   [ Html.div [ prop.className "flex flex-wrap"
                                prop.children (renderMainBody state dispatch) ] ] ]

let private view =
    fun (props: EditMealProps) ->
        let state, dispatch =
            React.useElmish (init props.MealId, update, [| box props.MealId |])

        renderBody state dispatch

let render =
    React.functionComponent ("EditMeal", view)
