module EditMeal.View

open Elmish
open Feliz
open Feliz.UseElmish
open System
open EditMeal.App

type EditMealProps = { MealId: Guid }

let private view =
    fun (props: EditMealProps) ->
        let state, dispatch =
            React.useElmish (init props.MealId, update, [| box props.MealId |])

        Html.text (sprintf "Edit Meal %A" state.MealId)

let render =
    React.functionComponent ("EditMeal", view)
