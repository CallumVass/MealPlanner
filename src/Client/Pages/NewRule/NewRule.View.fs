module NewRule.View

open System
open Elmish
open Feliz
open Feliz.UseElmish
open NewRule.App
open NewRule.Types

let private renderDay state dispatch (day: DayOfWeek) =

    let dayString = Enum.GetName(typeof<DayOfWeek>, day)

    let value =
        state.Rule.FormData.ApplicableOn
        |> List.tryFind (fun m -> m = day)
        |> Option.map (fun _ -> true)
        |> Option.defaultValue false

    let applyChange x day state =
        let newDays =
            match x with
            | true -> state.Rule.FormData.ApplicableOn @ [ day ]
            | false ->
                state.Rule.FormData.ApplicableOn
                |> List.filter (fun d -> d <> day)

        { state.Rule.FormData with
              ApplicableOn = newDays }

    Form.checkboxInput dayString (nameof state.Rule.FormData.ApplicableOn) value state.Rule.ValidationErrors (fun (x: bool) ->
        (applyChange x day state)
        |> FormChanged
        |> dispatch)

let private renderDays state dispatch days =
    Html.div [ prop.className "mt-6 px-3"
               prop.children (days |> List.map (renderDay state dispatch)) ]

let private renderDaysOfWeek state dispatch =
    match state.DaysOfWeek with
    | Resolved days -> days |> renderDays state dispatch
    | _ -> Html.none

let private renderForm state dispatch =

    let inputs =
        [ Form.textInput "Name" (nameof state.Rule.FormData.Name) state.Rule.FormData.Name state.Rule.ValidationErrors (fun x ->
              { state.Rule.FormData with Name = x }
              |> FormChanged
              |> dispatch)
          renderDaysOfWeek state dispatch ]

    let form =
        Html.div [ prop.className "flex flex-wrap pb-2"
                   prop.children inputs ]

    let createRuleButton =
        View.button (not state.Rule.ValidationErrors.IsEmpty) "Create Rule" (fun _ -> TrySave |> dispatch)

    Html.div [ prop.className "w-full mb-2"
               prop.children
                   [ (View.box [ View.h2 "New Rule"
                                 form
                                 createRuleButton ]) ] ]

let private view =
    fun () ->
        let state, dispatch = React.useElmish (init, update, [||])

        let children = [ renderForm state dispatch ]

        children |> View.renderBody

let render =
    React.functionComponent ("EditMeal", view)
