[<AutoOpen>]
module Extensions

open Elmish

let isDevelopment =
#if DEBUG
    true
#else
    false
#endif

type Deferred<'t> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 't

/// Utility functions around `Deferred<'T>` types.
module Deferred =
    let map (transform: 'T -> 'U) (deferred: Deferred<'T>): Deferred<'U> =
        match deferred with
        | HasNotStartedYet -> HasNotStartedYet
        | InProgress -> InProgress
        | Resolved value -> Resolved(transform value)

    /// Returns whether the `Deferred<'T>` value has been resolved or not.
    let resolved =
        function
        | HasNotStartedYet -> false
        | InProgress -> false
        | Resolved _ -> true

    /// Returns whether the `Deferred<'T>` value is in progress or not.
    let inProgress =
        function
        | HasNotStartedYet -> false
        | InProgress -> true
        | Resolved _ -> false

    /// Verifies that a `Deferred<'T>` value is resolved and the resolved data satisfies a given requirement.
    let exists (predicate: 'T -> bool) =
        function
        | HasNotStartedYet -> false
        | InProgress -> false
        | Resolved value -> predicate value

    /// Like `map` but instead of transforming just the value into another type in the `Resolved` case, it will transform the value into potentially a different case of the the `Deferred<'T>` type.
    let bind (transform: 'T -> Deferred<'U>) (deferred: Deferred<'T>): Deferred<'U> =
        match deferred with
        | HasNotStartedYet -> HasNotStartedYet
        | InProgress -> InProgress
        | Resolved value -> transform value

type AsyncOperationStatus<'t> =
    | Started
    | Finished of 't

module Log =
    /// Logs error to the console during development
    let developmentError (error: exn) =
        if isDevelopment then Browser.Dom.console.error (error)

module Cmd =
    /// Converts an asynchronous operation that returns a message into into a command of that message.
    /// Logs unexpected errors to the console while in development mode.
    let fromAsync (operation: Async<'msg>): Cmd<'msg> =
        let delayedCmd (dispatch: 'msg -> unit): unit =
            let delayedDispatch =
                async {
                    match! Async.Catch operation with
                    | Choice1Of2 msg -> dispatch msg
                    | Choice2Of2 error -> Log.developmentError error
                }

            Async.StartImmediate delayedDispatch

        Cmd.ofSub delayedCmd
