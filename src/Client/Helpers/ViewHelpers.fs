[<RequireQualifiedAccess>]
module View

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

let renderBody (children: ReactElement seq) =
    Html.div [ prop.className "flex flex-wrap mx-4"
               prop.children children ]
