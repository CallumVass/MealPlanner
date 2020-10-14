module Api

open Fable.Remoting.Client
open Shared.Types

let anonymousApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IAnonymousApi>

let mealApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IMealApi>
