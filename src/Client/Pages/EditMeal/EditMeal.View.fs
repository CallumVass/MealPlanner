module EditMeal.View

open Feliz
open System

type EditMealProps = { MealId: Guid }

let render =
    React.functionComponent
        ("EditMeal",
         (fun (props: EditMealProps) ->
             // let state, dispatch = React.useElmish (init, update, [||])
             Html.text (sprintf "Edit Meal %A" props.MealId)))
