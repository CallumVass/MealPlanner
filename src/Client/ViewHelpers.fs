[<RequireQualifiedAccess>]
module ViewHelpers

open Feliz

let baseClasses =
    "bg-green-500 text-white font-bold py-2 px-4 rounded"

let nonDisabledClasses =
    "hover:bg-green-700 focus:outline-none focus:shadow-outline"

let buttonLink (text: string) (href: string) =
    let combinedClasses = baseClasses + " " + nonDisabledClasses
    Html.a [ prop.href href
             prop.text text
             prop.className combinedClasses ]

let button isDisabled (text: string) onClick =

    let disabledClasses = "opacity-50 cursor-not-allowed"

    let combinedClasses =
        if isDisabled then baseClasses + " " + disabledClasses else baseClasses + " " + nonDisabledClasses

    Html.div [ prop.className "flex px-3"
               prop.children
                   [ Html.button [ prop.className combinedClasses
                                   prop.text text
                                   prop.disabled isDisabled
                                   prop.onClick onClick ] ] ]


let h2 (text: string) =
    Html.h2 [ prop.className
                  "bg-gradient-to-br from-purple-400 to-purple-700 flex p-2 mb-2 text-xl font-semibold text-white"
              prop.text text ]

let box (children: ReactElement seq) =
    Html.div [ prop.className "bg-white rounded-b pb-3"
               prop.children children ]

let private formInput (labelText: string) for' validationErrors input =
    Html.div [ prop.className "w-1/2 px-3"
               prop.children [ Html.label [ prop.className
                                                "block uppercase tracking-wide text-gray-700 text-xs font-bold mb-2"
                                            prop.htmlFor for'
                                            prop.text labelText ]
                               input
                               FormHelpers.errorMessage validationErrors for' ] ]

let private inputClasses =
    "appearance-none block w-full bg-gray-200 text-gray-700 border border-gray-200 rounded py-3 px-4 leading-tight focus:outline-none focus:bg-white focus:border-gray-500"

let textInput (labelText: string) (for': string) (inputValue: string) validationErrors (onChange: string -> unit) =
    let input =
        Html.input [ prop.className
                         (inputClasses
                          + FormHelpers.color validationErrors for')
                     prop.id for'
                     prop.type' "text"
                     prop.valueOrDefault inputValue
                     prop.onChange onChange ]

    input |> formInput labelText for' validationErrors

let numberInput (labelText: string) (for': string) (inputValue: int) validationErrors (onChange: string -> unit) =
    let input =
        Html.input [ prop.className
                         (inputClasses
                          + FormHelpers.color validationErrors for')
                     prop.id for'
                     prop.type' "number"
                     prop.valueOrDefault inputValue
                     prop.onChange onChange ]

    input |> formInput labelText for' validationErrors
