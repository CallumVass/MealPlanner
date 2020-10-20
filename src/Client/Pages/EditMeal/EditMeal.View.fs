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

let private renderDays (rule: Rule) =

    let days =
        rule.ApplicableOn
        |> List.map (fun d -> Enum.GetName(typeof<DayOfWeek>, d))
        |> String.concat ", "

    Html.p [ prop.text days
             prop.className "text-sm ml-6 mb-6" ]

let private renderRule (meal: ValidatedForm<Meal>) dispatch (rule: Rule) =
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
            (applyChange x rule meal)
            |> FormChanged
            |> dispatch)

    Html.div [ checkbox
               rule |> renderDays ]


let private renderRules rules dispatch meal =
    Html.div [ prop.className "mt-6 px-3"
               prop.children (rules |> List.map (renderRule meal dispatch)) ]

let private renderMeal dispatch rules meal =
    let inputs =
        [ Form.textInput "Name" (nameof meal.FormData.Name) meal.FormData.Name meal.ValidationErrors (fun x ->
              { meal.FormData with Name = x }
              |> FormChanged
              |> dispatch)
          meal |> renderRules rules dispatch ]

    let form =
        Html.div [ prop.className "flex pb-2"
                   prop.children inputs ]

    let editMealButton =
        View.button (not meal.ValidationErrors.IsEmpty) "Save" (fun _ -> (TrySave meal) |> dispatch)

    let formContainer =
        View.box [ (View.h2 "Meals")
                   form
                   editMealButton ]

    Html.div [ prop.className "w-full mb-2"
               prop.children formContainer ]

let private maybeRenderMeal dispatch rules (meal: ValidatedForm<Meal> option) =
    meal
    |> Option.map (renderMeal dispatch rules)
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
