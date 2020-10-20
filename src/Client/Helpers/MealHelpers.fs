[<RequireQualifiedAccess>]
module Meal

open System
open Feliz
open Form.Types
open Shared.Types

let private renderDays (rule: Rule) =

    let days =
        rule.ApplicableOn
        |> List.map (fun d -> Enum.GetName(typeof<DayOfWeek>, d))
        |> String.concat ", "

    Html.p [ prop.text days
             prop.className "text-sm ml-6 mb-6" ]

let private renderRule (meal: ValidatedForm<Meal>) dispatch msg (rule: Rule) =
    let value =
        meal.FormData.Rules
        |> List.tryFind (fun m -> m = rule)
        |> Option.map (fun _ -> true)
        |> Option.defaultValue false

    let applyChange x rule meal =
        let newRules =
            match x with
            | true -> meal.FormData.Rules @ [ rule ]
            | false ->
                meal.FormData.Rules
                |> List.filter (fun d -> d <> rule)

        { meal.FormData with Rules = newRules }

    let checkbox =
        Form.checkboxInput rule.Name (nameof meal.FormData.Rules) value meal.ValidationErrors (fun (x: bool) ->
            (applyChange x rule meal) |> msg |> dispatch)

    Html.div [ checkbox
               rule |> renderDays ]

let private renderRules rules dispatch msg meal =
    Html.div [ prop.className "mt-6 px-3"
               prop.children (rules |> List.map (renderRule meal dispatch msg)) ]

let render dispatch rules formChangeMsg formSaveMsg meal =
    let inputs =
        [ Form.textInput "Name" (nameof meal.FormData.Name) meal.FormData.Name meal.ValidationErrors (fun x ->
              { meal.FormData with Name = x }
              |> formChangeMsg
              |> dispatch)
          meal |> renderRules rules dispatch formChangeMsg ]

    let form =
        Html.div [ prop.className "flex pb-2"
                   prop.children inputs ]

    let editMealButton =
        View.button (not meal.ValidationErrors.IsEmpty) "Save" (fun _ -> (formSaveMsg meal) |> dispatch)

    let formContainer =
        View.box [ (View.h2 "Meals")
                   form
                   editMealButton ]

    Html.div [ prop.className "w-full mb-2"
               prop.children formContainer ]
