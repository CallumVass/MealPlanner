[<RequireQualifiedAccess>]
module Router

open Feliz.Router
open Main.Types

let activePage state =
    match state.CurrentUrl with
    | [] -> Home.View.render ()
    | [ "rules" ] -> RuleList.View.render ()
    | [ "categories" ] -> CategoryList.View.render ()
    | [ "meals"; Route.Guid mealId; "edit" ] -> EditMeal.View.render ({ MealId = mealId })
    | [ "categories"; Route.Guid categoryId; "edit" ] -> EditCategory.View.render ({ CategoryId = categoryId })
    | [ "rules"; Route.Guid ruleId; "edit" ] -> EditRule.View.render ({ RuleId = ruleId })
    | [ "rules"; "new" ] -> NewRule.View.render ()
    | [ "meals"; "new" ] -> NewMeal.View.render ()
    | [ "categories"; "new" ] -> NewCategory.View.render ()
    | _ -> Home.View.render ()
