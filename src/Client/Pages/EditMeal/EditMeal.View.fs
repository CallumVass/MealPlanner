module EditMeal.View

open Elmish
open Feliz
open Feliz.UseElmish
open System
open EditMeal.App
open EditMeal.Types
open Form.Types
open Shared.Types

type EditMealProps = { MealId: Guid }

let private maybeRenderMeal dispatch rules (meal: ValidatedForm<Meal> option) =
    meal
    |> Option.map (Meal.render dispatch rules FormChanged TrySave)
    |> Option.defaultValue Html.none

let private renderMainBody dispatch state =
    match state.Meal, state.Rules with
    | Resolved meal, Resolved rules -> meal |> maybeRenderMeal dispatch rules
    | _, _ -> Html.none

let private view =
    fun (props: EditMealProps) ->
        let state, dispatch =
            React.useElmish (init props.MealId, update, [| box props.MealId |])

        let children = [ (state |> renderMainBody dispatch) ]

        children |> View.renderBody

let render =
    React.functionComponent ("EditMeal", view)
