[<RequireQualifiedAccess>]
module Router

open Feliz.Router
open Main.Types

let activePage state =
    match state.CurrentUrl with
    | [] -> Home.View.render ()
    //    | [ "rules" ] -> RuleList.render ()
//    | [ "categories" ] -> CategoryList.render ()
    | [ "meals"; Route.Guid mealId; "edit" ] -> EditMeal.View.render ({ MealId = mealId })
    | [ "rules"; "new" ] -> NewRule.View.render ()
    | [ "meals"; "new" ] -> NewMeal.View.render ()
    | [ "categories"; "new" ] -> NewCategory.View.render ()
    | _ -> Home.View.render ()
