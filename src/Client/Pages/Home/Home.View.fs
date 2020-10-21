module Home.View

open System
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

let tryParseInt (s: string) d =
    match Int32.TryParse s with
    | true, int -> int
    | _ -> d

let private renderMainBody state dispatch =
    let form =
        Html.div [ prop.className "flex flex-wrap pb-2"
                   prop.children [ Form.numberInput "Days Between Same Meal"
                                       (nameof state.Options.FormData.DaysBetweenSameMeal)
                                       state.Options.FormData.DaysBetweenSameMeal state.Options.ValidationErrors (fun x ->
                                       { state.Options.FormData with
                                             DaysBetweenSameMeal = tryParseInt x 0 }
                                       |> FormChanged
                                       |> dispatch)
                                   Form.numberInput "Days To Calculate" (nameof state.Options.FormData.DaysToCalculate)
                                       state.Options.FormData.DaysToCalculate state.Options.ValidationErrors (fun x ->
                                       { state.Options.FormData with
                                             DaysToCalculate = tryParseInt x 0 }
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
            View.button
                (meals.IsEmpty
                 || not state.Options.ValidationErrors.IsEmpty) "Create Meal Plan" (fun _ -> Calculate |> dispatch)
        | _ -> View.button true "Create Meal Plan" (fun _ -> Calculate |> dispatch)

    Html.div [ prop.className "w-full sm:w-full md:w-3/5 pr-2 mb-2"
               prop.children [ (View.box [ View.h2 "Meals"
                                           form
                                           createMealPlanButton ])
                               mealPlan ] ]

let private renderMealItem meal =

    let actions =
        Html.div [ View.buttonLink "Edit" (Router.format ("meals", (sprintf "%A" meal.Id), "edit")) ]

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
                      prop.children
                          [ for meal in meals ->
                              Html.li [ prop.className "p-4 border-b"
                                        prop.children (renderMealItem meal) ] ] ]

        Html.div [ prop.className "w-full sm:w-full md:w-2/5 pl-2 mb-2"
                   prop.children
                       (View.box [ View.h2 "Available Meals"
                                   mealList ]) ]

let private view =
    fun () ->
        let state, dispatch = React.useElmish (init, update, [||])

        let children =
            [ renderMainBody state dispatch
              renderMealList state.AvailableMeals ]

        children |> View.renderBody

let render = React.functionComponent ("Index", view)
