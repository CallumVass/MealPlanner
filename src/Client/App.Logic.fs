module App.Logic

open App.Types
open Api
open Elmish

let parseUrl =
    function
    | [] -> Url.Index
    // | [ "meal"; Route.Int mealId; "edit" ] -> Url.EditMeal mealId
    | _ -> Url.Unknown

let performAuthCheck nextCmd =
    let authCheckCmd (dispatch: Msg -> unit): unit =
        let authCheck =
            async {
                let! isAuth = anonymousApi.IsAuthenticated()
                dispatch (AuthCheck(isAuth, nextCmd))
            }

        Async.StartImmediate authCheck

    Cmd.ofSub authCheckCmd

let checkIsAuth isAuth nextCmd state =
    if isAuth
    then { state with User = Authenticated }, nextCmd
    else state, Cmd.none
