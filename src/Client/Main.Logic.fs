module Main.Logic

open Main.Types
open Api
open Elmish

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
