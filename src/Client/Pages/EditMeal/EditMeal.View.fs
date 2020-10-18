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

let renderMeal dispatch meal =
    let input =
        FormHelpers.textInput "Name" (nameof meal.FormData.Name) meal.FormData.Name meal.ValidationErrors (fun x ->
            { meal.FormData with Name = x }
            |> FormChanged
            |> dispatch)

    let form =
        Html.div [ prop.className "flex pb-2"
                   prop.children input ]

    let editMealButton =
        ViewHelpers.button (not meal.ValidationErrors.IsEmpty) "Save" (fun _ -> (Save meal.FormData) |> dispatch)

    let formContainer =
        ViewHelpers.box [ (ViewHelpers.h2 "Meals")
                          form
                          editMealButton ]

    Html.div [ prop.className "w-full mb-2"
               prop.children formContainer ]

let private maybeRenderMeal dispatch (meal: ValidatedForm<Meal> option) =
    meal
    |> Option.map (renderMeal dispatch)
    |> Option.defaultValue Html.none

let private renderMainBody dispatch state =
    match state.Meal with
    | Resolved meal -> meal |> maybeRenderMeal dispatch
    | _ -> Html.none

let private view =
    fun (props: EditMealProps) ->
        let state, dispatch =
            React.useElmish (init props.MealId, update, [| box props.MealId |])

        let children = [ (state |> renderMainBody dispatch) ]

        children |> ViewHelpers.renderBody

let render =
    React.functionComponent ("EditMeal", view)
