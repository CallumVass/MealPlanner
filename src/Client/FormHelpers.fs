[<RequireQualifiedAccess>]
module FormHelpers

open Feliz
open Shared.Validation

let private errorMessage errors name =
    errors
    |> List.tryFind (fun x -> x.Field = name)
    |> Option.map (fun x ->
        Html.p [ prop.className "text-red-500 text-xs italic"
                 prop.text (x.Type |> ValidationErrorType.toString) ])
    |> Option.defaultValue Html.none

let private color errors name =
    errors
    |> List.tryFind (fun x -> x.Field = name)
    |> Option.map (fun _ -> " border border-red-500")
    |> Option.defaultValue ""

let private formInput (labelText: string) for' validationErrors input =
    Html.div [ prop.className "w-1/2 px-3"
               prop.children [ Html.label [ prop.className
                                                "block uppercase tracking-wide text-gray-700 text-xs font-bold mb-2"
                                            prop.htmlFor for'
                                            prop.text labelText ]
                               input
                               errorMessage validationErrors for' ] ]

let private inputClasses =
    "appearance-none block w-full bg-gray-200 text-gray-700 border border-gray-200 rounded py-3 px-4 leading-tight focus:outline-none focus:bg-white focus:border-gray-500"

let textInput (labelText: string) (for': string) (inputValue: string) validationErrors (onChange: string -> unit) =
    let input =
        Html.input [ prop.className (inputClasses + color validationErrors for')
                     prop.id for'
                     prop.type' "text"
                     prop.valueOrDefault inputValue
                     prop.onChange onChange ]

    input |> formInput labelText for' validationErrors

let numberInput (labelText: string) (for': string) (inputValue: int) validationErrors (onChange: string -> unit) =
    let input =
        Html.input [ prop.className (inputClasses + color validationErrors for')
                     prop.id for'
                     prop.type' "number"
                     prop.valueOrDefault inputValue
                     prop.onChange onChange ]

    input |> formInput labelText for' validationErrors
