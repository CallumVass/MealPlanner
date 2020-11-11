[<RequireQualifiedAccess>]
module Rule

open System
open Feliz
open Form.Types
open Shared.Types

let private renderDay dispatch rule formChangeMsg (day: DayOfWeek) =

    let dayString = Enum.GetName(typeof<DayOfWeek>, day)

    let value =
        rule.FormData.ApplicableOn
        |> List.tryFind (fun m -> m = day)
        |> Option.map (fun _ -> true)
        |> Option.defaultValue false

    let applyChange x day rule =
        let newDays =
            match x with
            | true -> rule.FormData.ApplicableOn @ [ day ]
            | false ->
                rule.FormData.ApplicableOn
                |> List.filter (fun d -> d <> day)

        { rule.FormData with
              ApplicableOn = newDays }

    Form.checkboxInput dayString value (fun (x: bool) ->
        (applyChange x day rule)
        |> formChangeMsg
        |> dispatch)

let private renderDays dispatch rule formChangeMsg days =
    let children =
        (days
         |> List.map (renderDay dispatch rule formChangeMsg))

    Html.div [ prop.className "mt-6 px-3"
               prop.children
                   (children
                    @ [ Form.errorMessage rule.ValidationErrors (nameof rule.FormData.ApplicableOn) ]) ]

let render dispatch daysOfWeek formChangeMsg formSaveMsg (rule: ValidatedForm<Rule>) =
    let inputs =
        [ Form.textInput "Name" (nameof rule.FormData.Name) rule.FormData.Name rule.ValidationErrors (fun x ->
              { rule.FormData with Name = x }
              |> formChangeMsg
              |> dispatch)
          daysOfWeek
          |> (renderDays dispatch rule formChangeMsg) ]

    let form =
        Html.div [ prop.className "flex flex-wrap pb-2"
                   prop.children inputs ]

    let createRuleButton =
        View.greenButton (not rule.ValidationErrors.IsEmpty) "Save" (fun _ -> formSaveMsg rule |> dispatch)

    Html.div [ prop.className "w-full mb-2"
               prop.children
                   [ (View.box [ View.h2 "Rules"
                                 form
                                 createRuleButton ]) ] ]
