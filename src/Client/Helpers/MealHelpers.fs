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
        Form.checkboxInput rule.Name value (fun (x: bool) -> (applyChange x rule meal) |> msg |> dispatch)

    Html.div [ checkbox
               rule |> renderDays ]

let private renderCategory (meal: ValidatedForm<Meal>) dispatch msg (category: MealCategory) =
    let value =
        meal.FormData.Category
        |> Option.map (fun c -> c = category)
        |> Option.defaultValue false

    let applyChange x category meal =
        match x with
        | true ->
            { meal.FormData with
                  Category = Some category }
        | false -> { meal.FormData with Category = None }

    let radio =
        Form.checkboxInput category.Name value (fun (x: bool) -> (applyChange x category meal) |> msg |> dispatch)

    Html.div [ radio ]

let private renderCategories (categories: MealCategory list) dispatch msg meal =
    if categories.IsEmpty then
        Html.none
    else
        let children =
            [ Html.p [ prop.className "block uppercase tracking-wide text-gray-700 text-xs font-bold mb-2"
                       prop.text "Category" ] ]
            @ (categories
               |> List.map (renderCategory meal dispatch msg))
            @ [ Form.errorMessage meal.ValidationErrors (nameof meal.FormData.Category) ]

        Html.div [ prop.className "mt-6 px-3 w-1/2"
                   prop.children children ]

let private renderRules (rules: Rule list) dispatch msg meal =
    if rules.IsEmpty then
        Html.none
    else
        let children =
            [ Html.p [ prop.className "block uppercase tracking-wide text-gray-700 text-xs font-bold mb-2"
                       prop.text "Rules" ] ]
            @ (rules |> List.map (renderRule meal dispatch msg))
            @ [ Form.errorMessage meal.ValidationErrors (nameof meal.FormData.Rules) ]

        Html.div [ prop.className "mt-6 px-3"
                   prop.children children ]

let render dispatch rules (categories: MealCategory list) formChangeMsg formSaveMsg meal =
    let inputs =
        [ Form.textInput "Name" (nameof meal.FormData.Name) meal.FormData.Name meal.ValidationErrors (fun x ->
              { meal.FormData with Name = x }
              |> formChangeMsg
              |> dispatch)
          meal
          |> renderCategories categories dispatch formChangeMsg
          meal |> renderRules rules dispatch formChangeMsg ]

    let form =
        Html.div [ prop.className "flex flex-wrap pb-2"
                   prop.children inputs ]

    let editMealButton =
        View.greenButton (not meal.ValidationErrors.IsEmpty) "Save" (fun _ -> (formSaveMsg meal) |> dispatch)

    let formContainer =
        View.box [ (View.h2 "Meals")
                   form
                   editMealButton ]

    Html.div [ prop.className "w-full mb-2"
               prop.children formContainer ]
