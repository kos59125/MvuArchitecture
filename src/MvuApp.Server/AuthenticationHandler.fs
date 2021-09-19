namespace MvuApp.Server

open System.Security.Claims
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Identity
open Bolero.Remoting
open Bolero.Remoting.Server
open MvuApp.Service.AuthenticationService


type AuthenticationHandler(remote:IRemoteContext, userRepository:IUserRepository) =
   inherit RemoteHandler<Service>()

   override _.Handler = {
      SignInAsync = fun req -> async {
         match userRepository.TryGetUser req.Name req.Password with
         | Some(user) ->
            let claims = [
               Claim(ClaimTypes.NameIdentifier, user.UserId.ToString("D"))
               Claim(ClaimTypes.Name, user.Name)
            ]
            let identity = ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
            let principal = ClaimsPrincipal(identity)
            do! remote.HttpContext.SignInAsync(principal) |> Async.AwaitTask
         | None ->
            return raise RemoteUnauthorizedException
      }

      SignOutAsync = fun () -> async {
         do! remote.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme) |> Async.AwaitTask
      }

      TryGetSignInUserAsync = fun () -> async {
         let user = remote.HttpContext.User
         match user.Identity.IsAuthenticated with
         | false ->
            return NotSignedIn
         | true ->
            let nameIdentifier = user.FindFirstValue(ClaimTypes.NameIdentifier)
            let name = user.FindFirstValue(ClaimTypes.Name)
            return User(nameIdentifier, name)
      }
   }
