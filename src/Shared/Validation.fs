module Shared.Validation

type ValidationErrorType =
    | IsEmpty
    | IsOutOfRange of int * int

module ValidationErrorType =
    let toString =
        function
        | IsEmpty -> "The value is required"
        | IsOutOfRange (min, max) -> sprintf "The value must be between %i and %i" min max

type ValidationError =
    { Field: string
      Type: ValidationErrorType }

let validateRange min max value =
    if value < min || value > max then IsOutOfRange(min, max) |> Some else None

let validate rules =
    List.map
        ((fun (n, e) -> n, Option.get e)
         >> (fun (f, t) -> { Field = f; Type = t }))
        (rules |> List.filter (snd >> Option.isSome))
