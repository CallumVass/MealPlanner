[<RequireQualifiedAccess>]
module Category

open Feliz
open Form.Types
open Shared.Types

let render dispatch formChangedMsg formSavedMsg (category: ValidatedForm<MealCategory>) =

    let inputs =
        [ Form.textInput "Name" (nameof category.FormData.Name) category.FormData.Name category.ValidationErrors (fun x ->
              { category.FormData with Name = x }
              |> formChangedMsg
              |> dispatch) ]

    let form =
        Html.div [ prop.className "flex flex-wrap pb-2"
                   prop.children inputs ]

    let addCategoryButton =
        View.greenButton (not category.ValidationErrors.IsEmpty) "Save" (fun _ -> formSavedMsg category |> dispatch)

    let formContainer =
        View.box [ (View.h2 "Categories")
                   form
                   addCategoryButton ]

    Html.div [ prop.className "w-full mb-2"
               prop.children formContainer ]
