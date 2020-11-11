module EditRule.View

open Elmish
open Feliz
open Feliz.UseElmish
open System
open EditRule.App
open EditRule.Types
open Form.Types
open Shared.Types

type EditRuleProps = { RuleId: Guid }

let private maybeRenderRule dispatch daysOfWeek (rule: ValidatedForm<Rule> option) =
    rule
    |> Option.map (Rule.render dispatch daysOfWeek FormChanged TrySave)
    |> Option.defaultValue Html.none

let private renderMainBody dispatch state =
    match state.Rule, state.DaysOfWeek with
    | Resolved rule, Resolved daysOfWeek -> rule |> maybeRenderRule dispatch daysOfWeek
    | _, _ -> Html.none

let private view =
    fun (props: EditRuleProps) ->
        let state, dispatch =
            React.useElmish (init props.RuleId, update, [| box props.RuleId |])

        let children = [ (state |> renderMainBody dispatch) ]

        children |> View.renderBody

let render =
    React.functionComponent ("EditRule", view)
