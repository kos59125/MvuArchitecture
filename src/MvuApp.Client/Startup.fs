namespace MvuApp.Client

open Microsoft.AspNetCore.Components.WebAssembly.Hosting
open Bolero.Remoting.Client

module Program =

   [<EntryPoint>]
   [<CompiledName("Main")>]
   let main args =
      let builder = WebAssemblyHostBuilder.CreateDefault(args)
      builder.RootComponents.Add<Application.Application.ApplicationComponent>("#main")
      builder.Services.AddRemoting(builder.HostEnvironment) |> ignore
      builder.Build().RunAsync() |> ignore
      0
