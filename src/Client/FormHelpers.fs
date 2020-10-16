[<RequireQualifiedAccess>]
module FormHelpers

open Feliz
open Shared.Validation

let errorMessage errors name =
    errors
    |> List.tryFind (fun x -> x.Field = name)
    |> Option.map (fun x ->
        Html.p [ prop.className "text-red-500 text-xs italic"
                 prop.text (x.Type |> ValidationErrorType.toString) ])
    |> Option.defaultValue Html.none

let color errors name =
    errors
    |> List.tryFind (fun x -> x.Field = name)
    |> Option.map (fun _ -> " border border-red-500")
    |> Option.defaultValue ""
