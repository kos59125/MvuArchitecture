namespace MvuApp.Server

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Components.Authorization
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Bolero.Remoting.Server
open Bolero.Templating.Server
open Bolero.Server

type Startup() =

   member __.ConfigureServices(services:IServiceCollection) =
      services.AddMvc().AddRazorRuntimeCompilation() |> ignore
      services.AddServerSideBlazor() |> ignore
      services
         .AddAuthorization()
         .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie()
            .Services
         |> ignore
      services
         .AddSingleton<IUserRepository, UserRepository>()
         .AddRemoting<AuthenticationHandler>()
         .AddRemoting<RemoteCounterHandler>()
         .AddBoleroHost()
#if DEBUG
         .AddHotReload(templateDir = __SOURCE_DIRECTORY__ + "/../MvuApp.Client")
#endif
      |> ignore

   member __.Configure(app:IApplicationBuilder, env:IWebHostEnvironment) =
      app
         .UseAuthentication()
         .UseRemoting()
         .UseStaticFiles()
         .UseRouting()
         .UseBlazorFrameworkFiles()
         .UseEndpoints(fun endpoints ->
#if DEBUG
            endpoints.UseHotReload()
#endif
            endpoints.MapBlazorHub() |> ignore
            endpoints.MapFallbackToPage("/_Host") |> ignore)
      |> ignore

module Program =

   [<EntryPoint>]
   let main args =
      WebHost
         .CreateDefaultBuilder(args)
         .UseStaticWebAssets()
         .UseStartup<Startup>()
         .Build()
         .Run()
      0
