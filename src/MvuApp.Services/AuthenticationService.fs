module MvuApp.Service.AuthenticationService

open Bolero.Remoting

type SignInRequestModel = {
   Name : string
   Password : string
}

type SignInUserResponseModel =
   | NotSignedIn
   | User of string * string

type Service = {
   SignInAsync : SignInRequestModel -> Async<unit>
   SignOutAsync : unit -> Async<unit>
   TryGetSignInUserAsync : unit -> Async<SignInUserResponseModel>
} with
   interface IRemoteService with
      member _.BasePath = "/-/authentication"
