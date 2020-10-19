module Shared.Validation

open System

type ValidationErrorType =
    | IsEmpty
    | ListIsEmpty
    | IsOutOfRange of int * int

module ValidationErrorType =
    let toString =
        function
        | IsEmpty -> "The value is required"
        | ListIsEmpty -> "The list must contain at least 1 value"
        | IsOutOfRange (min, max) -> sprintf "The value must be between %i and %i" min max

type ValidationError =
    { Field: string
      Type: ValidationErrorType }

let validateRange min max value =
    if value < min || value > max then IsOutOfRange(min, max) |> Some else None

let validateNotEmpty value =
    if String.IsNullOrEmpty value then IsEmpty |> Some else None

let validateListNotEmpty value =
    if value |> List.isEmpty then ListIsEmpty |> Some else None

let validate rules =
    List.map
        ((fun (n, e) -> n, Option.get e)
         >> (fun (f, t) -> { Field = f; Type = t }))
        (rules |> List.filter (snd >> Option.isSome))
