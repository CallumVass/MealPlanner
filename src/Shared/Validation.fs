module Shared.Validation

type ValidationErrorType =
    | IsEmpty
    | IsBelowMinimalLength of int
    | IsAboveMaximumLength of int

module ValidationErrorType =
    let explain =
        function
        | IsEmpty -> "The value is required"
        | IsBelowMinimalLength l -> sprintf "The value must be at least %i" l
        | IsAboveMaximumLength l -> sprintf "The value must not be more than %i" l

type ValidationError =
    { Field: string
      Type: ValidationErrorType }
