module Shared.Rule.Validation

open Shared.Validation
open Shared.Types

let validateRule (rule: Rule) =
    [ (nameof rule.Name), (validateNotEmpty rule.Name)
      (nameof rule.ApplicableOn), (validateListNotEmpty rule.ApplicableOn) ]
    |> validate
