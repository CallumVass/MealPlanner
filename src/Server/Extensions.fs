[<AutoOpen>]
module Extensions

open Saturn
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Authentication
open System
open Microsoft.Extensions.Configuration

[<RequireQualifiedAccess>]
module Seq =
    let inline asyncMap fn v = v |> Seq.map (fn) |> Async.Parallel

[<RequireQualifiedAccess>]
module Option =
    let inline asyncApply fn v =
        let executeSome fn v =
            async {
                let! result = v |> fn
                return Some result
            }

        async {
            match v with
            | Some m -> return! m |> executeSome fn
            | None -> return None
        }

type Microsoft.FSharp.Control.AsyncBuilder with
    member this.Bind(task, f) = this.Bind(Async.AwaitTask task, f)

    member this.Bind(tasks, f) =
        tasks |> List.map (fun t -> this.Bind(t, f))

let private addCookie state (c: AuthenticationBuilder) =
    if not state.CookiesAlreadyAdded then c.AddCookie() |> ignore

type Saturn.Application.ApplicationBuilder with
    [<CustomOperation("use_google_oauth_from_config")>]
    member __.UseGoogleAuthFromConfig(state: ApplicationState,
                                      callbackPath: string,
                                      jsonToClaimMap: (string * string) seq) =
        let middleware (app: IApplicationBuilder) = app.UseAuthentication()

        let service (s: IServiceCollection) =
            let c =
                s.AddAuthentication(fun cfg ->
                    cfg.DefaultScheme <- CookieAuthenticationDefaults.AuthenticationScheme
                    cfg.DefaultSignInScheme <- CookieAuthenticationDefaults.AuthenticationScheme
                    cfg.DefaultChallengeScheme <- "Google")

            let sp = s.BuildServiceProvider()

            let config = sp.GetService<IConfiguration>()

            addCookie state c
            c.AddGoogle(fun opt ->
                opt.ClientId <- config.GetValue("Google:ClientId")
                opt.ClientSecret <- config.GetValue("Google:ClientSecret")
                opt.CallbackPath <- PathString(callbackPath)
                jsonToClaimMap
                |> Seq.iter (fun (k, v) -> opt.ClaimActions.MapJsonKey(v, k))
                opt.ClaimActions.MapJsonSubKey("urn:google:image:url", "image", "url")
                opt.CorrelationCookie.SameSite <- SameSiteMode.Lax
                let ev = opt.Events

                ev.OnCreatingTicket <- Func<_, _> parseAndValidateOauthTicket

                )
            |> ignore
            s

        { state with
              ServicesConfig = service :: state.ServicesConfig
              AppConfigs = middleware :: state.AppConfigs
              CookiesAlreadyAdded = true }
