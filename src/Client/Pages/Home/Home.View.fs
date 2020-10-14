module Home.View

open Fable.Core.JsInterop
open Feliz
open Feliz.Router
open Feliz.UseElmish
open Elmish
open Home.App
open Home.Types
open Shared.Types

let private renderMeal (day, date, meal) =
    Html.div [ Html.p (sprintf "Day: %s Meal: %s" day meal.Name) ]

let private renderCalculatedMeals dailyMeals =
    Html.div [ prop.className "p-2"
               prop.children (dailyMeals |> List.map renderMeal) ]

let private renderMainBody state dispatch =
    let formInput (labelText: string) for' (inputValue: int) msg =
        Html.div [ prop.className "w-1/2 px-3"
                   prop.children [ Html.label [ prop.className
                                                    "block uppercase tracking-wide text-gray-700 text-xs font-bold mb-2"
                                                prop.htmlFor for'
                                                prop.text labelText ]
                                   Html.input [ prop.className
                                                    "appearance-none block w-full bg-gray-200 text-gray-700 border border-gray-200 rounded py-3 px-4 leading-tight focus:outline-none focus:bg-white focus:border-gray-500"
                                                prop.id for'
                                                prop.type' "number"
                                                prop.valueOrDefault inputValue
                                                prop.onChange (fun (ev: Browser.Types.Event) ->
                                                    (msg ev.target?value) |> dispatch) ] ] ]

    let form =
        Html.div [ prop.className "flex pb-2"
                   prop.children [ formInput
                                       "Days Between Same Meal"
                                       "days-between-meal"
                                       state.Options.DaysBetweenSameMeal
                                       ChangeDaysBetweenSameMeal
                                   formInput
                                       "Days To Calculate"
                                       "days-to-calculate"
                                       state.Options.DaysToCalculate
                                       ChangeDaysToCalculate ] ]

    let mealPlan =
        if state.ChosenMeals.IsEmpty then
            Html.none
        else
            Html.div [ prop.className "mt-2"
                       prop.children
                           (ViewHelpers.box [ ViewHelpers.h2 "Meal Plan"
                                              renderCalculatedMeals state.ChosenMeals ]) ]

    let createMealPlanButton =
        match state.AvailableMeals with
        | Resolved meals -> ViewHelpers.button meals.IsEmpty "Create Meal Plan" (fun _ -> Calculate |> dispatch)
        | _ -> ViewHelpers.button true "Create Meal Plan" (fun _ -> Calculate |> dispatch)

    Html.div [ prop.className "w-full sm:w-full md:w-3/5 px-2 mb-2"
               prop.children [ (ViewHelpers.box [ ViewHelpers.h2 "Meals"
                                                  form
                                                  createMealPlanButton ])
                               mealPlan ] ]

let private renderMealItem meal =

    let actions =
        Html.div [ ViewHelpers.buttonLink "Edit" (Router.format ("meals", (sprintf "%A" meal.Id), "edit")) ]

    Html.div [ prop.className "flex justify-between items-center"
               prop.children [ Html.text meal.Name
                               actions ] ]

let private renderMealList availableMeals =

    match availableMeals with
    | HasNotStartedYet -> Html.none
    | InProgress -> Html.none
    | Resolved meals ->
        let mealList =
            Html.ul [ prop.className "flex flex-col text-sm"
                      prop.children [ for meal in meals ->
                                          Html.li [ prop.className "p-4 border-b"
                                                    prop.children (renderMealItem meal) ] ] ]

        Html.div [ prop.className "w-full sm:w-full md:w-2/5 px-2 mb-2"
                   prop.children
                       (ViewHelpers.box [ ViewHelpers.h2 "Available Meals"
                                          mealList ]) ]

let private renderBody state dispatch =
    let children =
        [ renderMainBody state dispatch
          renderMealList state.AvailableMeals ]

    Html.div [ prop.className "px-2"
               prop.children [ Html.div [ prop.className "flex flex-wrap"
                                          prop.children children ] ] ]

let private view =
    fun () ->
        let state, dispatch = React.useElmish (init, update, [||])
        renderBody state dispatch

let render = React.functionComponent ("Index", view)
