module EditCategory.View

open Elmish
open Feliz
open Feliz.UseElmish
open System
open EditCategory.App
open EditCategory.Types
open Form.Types
open Shared.Types

type EditCategoryProps = { CategoryId: Guid }

let private maybeRenderCategory dispatch (category: ValidatedForm<MealCategory> option) =
    category
    |> Option.map (Category.render dispatch FormChanged TrySave)
    |> Option.defaultValue Html.none

let private renderMainBody dispatch (state: State) =
    match state.Category with
    | Resolved category -> category |> maybeRenderCategory dispatch
    | _ -> Html.none

let private view =
    fun (props: EditCategoryProps) ->
        let state, dispatch =
            React.useElmish (init props.CategoryId, update, [| box props.CategoryId |])

        let children = [ (state |> renderMainBody dispatch) ]

        children |> View.renderBody

let render =
    React.functionComponent ("EditCategory", view)
