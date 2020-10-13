module EditMeal.App

open Elmish
open EditMeal.Types
open Shared
open System
open Api
open Index.Logic

let defaultState = { Id = "hello" }

let init mealId =
    defaultState, Cmd.OfAsync.perform mealApi.GetMeals () GotMeals

let update msg state = state, Cmd.none
