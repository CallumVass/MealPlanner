module CategoryList.App

open Api
open Elmish
open CategoryList.Types
open Shared.Types

let defaultState = { Categories = HasNotStartedYet }

let init () =
    defaultState, Cmd.ofMsg (GetCategories Started)

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | DeleteCategory categoryId ->
        let deleteCategory =
            async {
                let! _ = mealApi.DeleteCategory categoryId
                return GetCategories Started
            }

        state, Cmd.fromAsync deleteCategory
    | GetCategories Started ->
        let loadCategories =
            async {
                let! categories = mealApi.GetCategories()
                return GetCategories(Finished categories)
            }

        { state with Categories = InProgress }, Cmd.fromAsync loadCategories
    | GetCategories (Finished categories) ->
        { state with
              Categories = Resolved categories },
        Cmd.none
