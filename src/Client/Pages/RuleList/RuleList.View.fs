module RuleList.View

open Feliz
open Feliz.UseElmish
open Feliz.Router
open RuleList.App
open RuleList.Types
open Shared.Types

let private renderRuleItem dispatch (rule: Rule) =

    let actions =
        Html.div [ prop.className "flex"
                   prop.children [ View.buttonLink "Edit" (Router.format ("rules", (sprintf "%A" rule.Id), "edit"))
                                   View.enabledRedButton "Delete" (fun _ -> DeleteRule rule.Id |> dispatch) ] ]

    Html.div [ prop.className "flex justify-between items-center"
               prop.children [ Html.text rule.Name
                               actions ] ]

let private linkContainer (link: ReactElement) =
    Html.div [ prop.className "mr-4 mt-3 flex justify-end"
               prop.children link ]

let private renderRuleList dispatch rules =

    match rules with
    | HasNotStartedYet -> Html.none
    | InProgress -> Html.none
    | Resolved rules ->
        let ruleList =
            Html.ul [ prop.className "flex flex-col text-sm"
                      prop.children
                          [ for rule in rules ->
                              Html.li [ prop.className "p-4 border-b"
                                        prop.children (rule |> renderRuleItem dispatch) ] ] ]

        Html.div [ prop.className "w-full mb-2"
                   prop.children
                       (View.box [ View.h2 "Available Rules"
                                   linkContainer (View.buttonLink "Add Rule" (Router.format ("rules", "new")))
                                   ruleList ]) ]

let private view =
    fun () ->
        let state, dispatch = React.useElmish (init, update, [||])

        let children =
            [ state.Rules |> renderRuleList dispatch ]

        children |> View.renderBody

let render =
    React.functionComponent ("RuleList", view)
