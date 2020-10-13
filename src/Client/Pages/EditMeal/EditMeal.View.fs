module EditMeal.View

open Elmish
open Feliz
open Feliz.UseElmish
open System
open EditMeal.App

type EditMealProps = { MealId: Guid }

let render =
    React.functionComponent
        ("EditMeal",
         (fun (props: EditMealProps) ->

             let state, dispatch =
                 React.useElmish (init props.MealId, update, [||])

             Html.text (sprintf "Edit Meal %A" state.MealId)))
