[<RequireQualifiedAccess>]
module View

open Feliz

type Colour =
    | Green
    | Red

let baseClasses colour =
    let buttonColour =
        match colour with
        | Green -> "bg-green-500"
        | Red -> "bg-red-500"

    "block text-white font-bold py-2 px-4 rounded"
    + " "
    + buttonColour

let nonDisabledClasses colour =
    let hoverColour =
        match colour with
        | Green -> "hover:bg-green-700"
        | Red -> "hover:bg-red-700"

    "focus:outline-none focus:shadow-outline"
    + " "
    + hoverColour

let buttonLink (text: string) (href: string) =
    let combinedClasses =
        (baseClasses Green)
        + " "
        + (nonDisabledClasses Green)

    Html.a [ prop.href href
             prop.text text
             prop.className combinedClasses ]

let private button isDisabled (text: string) colour onClick =

    let disabledClasses = "opacity-50 cursor-not-allowed"

    let combinedClasses =
        if isDisabled then
            baseClasses colour + " " + disabledClasses
        else
            baseClasses colour
            + " "
            + (nonDisabledClasses colour)

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

let renderBody (children: ReactElement seq) =
    Html.div [ prop.className "flex flex-wrap mx-4"
               prop.children children ]

let greenButton isDisabled (text: string) onClick = button isDisabled text Green onClick
let enabledGreenButton (text: string) onClick = button false text Green onClick
let disabledGreenButton (text: string) onClick = button true text Green onClick
let enabledRedButton (text: string) onClick = button false text Red onClick
