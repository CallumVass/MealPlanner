module Home.View

open System
open Feliz
open Feliz.Router
open Feliz.UseElmish
open Elmish
open Home.App
open Home.Types
open Shared.Types

let private renderMeal (day, date, meal: Meal option) =
    let buildChild (text: string) (value: string) =
        Html.p [ prop.className "border-b mb-2 pb-2"
                 prop.children [ Html.span [ prop.className "font-semibold"
                                             prop.text text ]
                                 Html.text value ] ]


    Html.div [ prop.className "grid grid-cols-3"
               prop.children [ buildChild "Day:" (sprintf " %s" day)
                               buildChild "Date:" (sprintf " %s" date)
                               buildChild
                                   "Meal:"
                                   (sprintf
                                       " %s"
                                        (meal
                                         |> Option.map (fun m -> m.Name)
                                         |> Option.defaultValue "No Applicable Meal Found")) ] ]

let private renderCalculatedMeals dailyMeals =
    Html.div [ prop.className "p-2"
               prop.children (dailyMeals |> List.map renderMeal) ]

let tryParseInt (s: string) d =
    match Int32.TryParse s with
    | true, int -> int
    | _ -> d

let tryParseDate (s: string) d =
    match DateTime.TryParse(s) with
    | true, date -> date
    | _ -> d

let private renderMainBody state dispatch =
    let form =
        Html.div [ prop.className "flex flex-wrap pb-2"
                   prop.children [ Form.numberInput "Days Between Same Meal"
                                       (nameof state.Options.FormData.DaysBetweenSameMeal) (Some 0)
                                       state.Options.FormData.DaysBetweenSameMeal state.Options.ValidationErrors (fun x ->
                                       { state.Options.FormData with
                                             DaysBetweenSameMeal = tryParseInt x 0 }
                                       |> FormChanged
                                       |> dispatch)
                                   Form.numberInput "Days Between Same Meal Category"
                                       (nameof state.Options.FormData.DaysBetweenSameMealCategory) (Some 0)
                                       state.Options.FormData.DaysBetweenSameMealCategory state.Options.ValidationErrors (fun x ->
                                       { state.Options.FormData with
                                             DaysBetweenSameMealCategory = tryParseInt x 0 }
                                       |> FormChanged
                                       |> dispatch)
                                   Form.numberInput "Days To Calculate" (nameof state.Options.FormData.DaysToCalculate)
                                       (Some 1) state.Options.FormData.DaysToCalculate state.Options.ValidationErrors (fun x ->
                                       { state.Options.FormData with
                                             DaysToCalculate = tryParseInt x 0 }
                                       |> FormChanged
                                       |> dispatch)
                                   Form.dateInput "Start Date" (nameof state.Options.FormData.FromDate)
                                       state.Options.FormData.FromDate state.Options.ValidationErrors (fun x ->
                                       { state.Options.FormData with
                                             FromDate =
                                                 (DateTime.Now.AddDays(float 1).Date
                                                  |> tryParseDate x) }
                                       |> FormChanged
                                       |> dispatch) ] ]

    let mealPlan =
        if state.ChosenMeals.IsEmpty then
            Html.none
        else
            Html.div [ prop.className "mt-2"
                       prop.children
                           (View.box [ View.h2 "Meal Plan"
                                       renderCalculatedMeals state.ChosenMeals ]) ]

    let createMealPlanButton =
        match state.AvailableMeals with
        | Resolved meals ->
            View.greenButton
                (meals.IsEmpty
                 || not state.Options.ValidationErrors.IsEmpty) "Create Meal Plan" (fun _ -> Calculate |> dispatch)
        | _ -> View.disabledGreenButton "Create Meal Plan" (fun _ -> Calculate |> dispatch)

    Html.div [ prop.className "w-full sm:w-full md:w-3/5 pr-0 md:pr-2 mb-4"
               prop.children [ (View.box [ View.h2 "Meals"
                                           form
                                           createMealPlanButton ])
                               mealPlan ] ]

let private renderMealItem dispatch meal =

    let actions =
        Html.div [ prop.className "flex"
                   prop.children [ View.buttonLink "Edit" (Router.format ("meals", (sprintf "%A" meal.Id), "edit"))
                                   View.enabledRedButton "Delete" (fun _ -> DeleteMeal meal.Id |> dispatch) ] ]

    Html.div [ prop.className "flex justify-between items-center"
               prop.children [ Html.text meal.Name
                               actions ] ]

let private renderMealList dispatch availableMeals =

    match availableMeals with
    | HasNotStartedYet -> Html.none
    | InProgress -> Html.none
    | Resolved meals ->
        let mealList =
            Html.ul [ prop.className "flex flex-col text-sm"
                      prop.children
                          [ for meal in meals ->
                              Html.li [ prop.className "p-4 border-b"
                                        prop.children (meal |> renderMealItem dispatch) ] ] ]

        Html.div [ prop.className "w-full sm:w-full md:w-2/5 pl-0 md:pl-2 mb-4"
                   prop.children
                       (View.box [ View.h2 "Available Meals"
                                   mealList ]) ]

let private view =
    fun () ->
        let state, dispatch = React.useElmish (init, update, [||])

        let children =
            [ renderMainBody state dispatch
              state.AvailableMeals |> renderMealList dispatch ]

        children |> View.renderBody

let render = React.functionComponent ("Index", view)
