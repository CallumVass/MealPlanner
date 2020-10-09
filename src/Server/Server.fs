module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open Shared
open Storage
open System.Security.Claims
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Serilog
open Giraffe.Core
open Giraffe.Auth
open Giraffe.Routing
open Giraffe.ResponseWriters

let mealApi userId (storage: MealStorage) =
    { GetMeals = fun () -> async { return! storage.GetMeals userId } }

let createApi mealApi (context: HttpContext) =
    let claim =
        context.User.FindFirst(ClaimTypes.NameIdentifier)

    let storage = context.GetService<MealStorage>()

    storage |> mealApi claim.Value

let webApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromContext (createApi mealApi)
    |> Remoting.buildHttpHandler

let mustBeLoggedIn =
    requiresAuthentication (challenge "Google")

let authRoutes =
    choose [ route "/" >=> htmlFile "public/index.html"
             webApi ]

let webApp = choose [ mustBeLoggedIn >=> authRoutes ]

let callbackPath = "/signin-google"

let jsonMappings =
    [ "id", ClaimTypes.NameIdentifier
      "displayName", ClaimTypes.Name ]

let configureHost (hostBuilder: IHostBuilder) =
    hostBuilder.UseSerilog(fun _ configureLogger ->
        configureLogger.MinimumLevel.Information().Enrich.FromLogContext().WriteTo.Console()
        |> ignore)
    |> ignore

    hostBuilder.ConfigureAppConfiguration(fun ctx builder ->
        builder.AddJsonFile("appSettings.json") |> ignore
        builder.AddJsonFile(sprintf "appSettings.%s.json" ctx.HostingEnvironment.EnvironmentName)
        |> ignore)

let configureServices (serviceCollection: IServiceCollection) =
    serviceCollection.AddSingleton<Migrator>().AddScoped<MealStorage>()

OptionHandler.register ()

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        service_config configureServices
        memory_cache
        use_google_oauth_from_config callbackPath jsonMappings
        use_static "public"
        use_gzip
        host_config configureHost
    }

[<EntryPoint>]
let main _ =

    let webHost = app.Build()
    use scope = webHost.Services.CreateScope()

    let migrator =
        scope.ServiceProvider.GetRequiredService<Migrator>()

    migrator.Migrate()
    webHost.Run()

    0
