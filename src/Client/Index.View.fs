module Index.View

open Fable.Core.JsInterop
open Feliz
open Index
open Index.Domain
open Shared

let renderMeal (day, date, meal) =
    Html.div [ Html.p (sprintf "Day: %s Meal: %s" day meal.Name) ]

let renderCalculatedMeals dailyMeals =
    Html.div [ prop.className "p-2"
               prop.children (dailyMeals |> List.map renderMeal) ]

let renderHeader =
    let h1 =
        Html.h1 [ prop.className "text-3xl font-semibold text-white"
                  prop.text "Meal Planner" ]

    Html.div [ prop.className "bg-gradient-to-br from-purple-400 to-purple-700 flex p-4 mb-6"
               prop.children h1 ]

let h2 (text: string) =
    Html.h2 [ prop.className
                  "bg-gradient-to-br from-purple-400 to-purple-700 flex p-2 mb-2 text-xl font-semibold text-white"
              prop.text text ]

let box (children: ReactElement seq) =
    Html.div [ prop.className "bg-white rounded-b pb-3"
               prop.children children ]

let button isDisabled (text: string) onClick =

    let baseClasses =
        "bg-green-500 text-white font-bold py-2 px-4 rounded"

    let nonDisabledClasses =
        "hover:bg-green-700 focus:outline-none focus:shadow-outline"

    let disabledClasses = "opacity-50 cursor-not-allowed"

    let combinedClasses =
        if isDisabled then baseClasses + " " + disabledClasses else baseClasses + " " + nonDisabledClasses

    Html.div [ prop.className "flex px-3"
               prop.children [ Html.button [ prop.className combinedClasses
                                             prop.text text
                                             prop.disabled isDisabled
                                             prop.onClick onClick ] ] ]

let renderMainBody state dispatch =

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
                           (box [ h2 "Meal Plan"
                                  renderCalculatedMeals state.ChosenMeals ]) ]

    let createMealPlanButton =
        button state.AvailableMeals.IsEmpty "Create Meal Plan" (fun _ -> Calculate |> dispatch)

    Html.div [ prop.className "w-full sm:w-full md:w-3/5 px-2 mb-2"
               prop.children [ (box [ h2 "Meals"
                                      form
                                      createMealPlanButton ])
                               mealPlan ] ]

let renderMealList meals dispatch =

    let mealList =
        Html.ul [ prop.className "flex items-center flex-col"
                  prop.children [ for meal in meals ->
                                      Html.li [ prop.className "mb-2"
                                                prop.text meal.Name ] ] ]

    let addMealButton =
        button false "Add Meal" (fun _ -> AddMeal |> dispatch)

    Html.div [ prop.className "w-full sm:w-full md:w-2/5 px-2 mb-2"
               prop.children
                   (box [ h2 "Available Meals"
                          mealList
                          addMealButton ]) ]

let renderBody state dispatch =
    Html.div [ prop.className "px-2"
               prop.children [ Html.div [ prop.className "flex flex-wrap"
                                          prop.children [ renderMainBody state dispatch
                                                          renderMealList state.AvailableMeals dispatch ] ] ] ]

let view state dispatch =

    let children =
        [ renderHeader
          renderBody state dispatch ]

    Html.div [ prop.className "min-h-screen bg-gray-200 text-gray-800"
               prop.children children ]
