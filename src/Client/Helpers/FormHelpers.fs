[<RequireQualifiedAccess>]
module Form

open System
open Feliz
open Shared.Validation

let private color errors name =
    errors
    |> List.tryFind (fun x -> x.Field = name)
    |> Option.map (fun _ -> " border border-red-500")
    |> Option.defaultValue ""

let private labelClasses =
    "block uppercase tracking-wide text-gray-700 text-xs font-bold mb-2"

let errorMessage errors name =
    errors
    |> List.tryFind (fun x -> x.Field = name)
    |> Option.map (fun x ->
        Html.p [ prop.className "text-red-500 text-xs italic"
                 prop.text (x.Type |> ValidationErrorType.toString) ])
    |> Option.defaultValue Html.none

let private formInput (labelText: string) for' validationErrors input =
    Html.div [ prop.className "w-full md:w-1/2 px-3 mt-4"
               prop.children [ Html.label [ prop.className labelClasses
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

let dateInput (labelText: string) (for': string) (inputValue: DateTime) validationErrors (onChange: string -> unit) =
    let dateAsString = inputValue.ToString("yyyy-MM-dd")

    let input =
        Html.input [ prop.className (inputClasses + color validationErrors for')
                     prop.id for'
                     prop.type' "date"
                     prop.min (DateTime.Now.AddDays(float 1))
                     prop.valueOrDefault dateAsString
                     prop.onChange onChange ]

    input |> formInput labelText for' validationErrors

let numberInput (labelText: string)
                (for': string)
                (minValue: int option)
                (inputValue: int)
                validationErrors
                (onChange: string -> unit)
                =
    let props =
        [ prop.className (inputClasses + color validationErrors for')
          prop.id for'
          prop.type' "number"
          prop.valueOrDefault inputValue
          prop.onChange onChange ]

    let props =
        match minValue with
        | Some i -> props @ [ prop.min i ]
        | None -> props

    let input = Html.input props

    input |> formInput labelText for' validationErrors

let checkboxInput (labelText: string) (inputValue: bool) (onChange: bool -> unit) =
    Html.label [ prop.className labelClasses
                 prop.htmlFor labelText
                 prop.children [ Html.input [ prop.className "mr-2 leading-tight"
                                              prop.id labelText
                                              prop.type' "checkbox"
                                              prop.isChecked inputValue
                                              prop.onChange onChange ]
                                 Html.span [ prop.className "text-sm"
                                             prop.text labelText ] ] ]
