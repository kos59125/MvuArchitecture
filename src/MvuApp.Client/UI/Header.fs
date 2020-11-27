module MvuApp.Client.UI.Header

open Bolero

type UserInfo = {
   Name : string
}

type Model = {
   User : UserInfo option
}

type Message =
   | UserSignedIn of UserInfo
   | UserSignedOut
   | SignInButtonClicked
   | SignOutButtonClicked

let init () =
   {
      User = None
   }

let update message model =
   match message with
   | UserSignedIn(user) -> { model with User = Some(user) }
   | UserSignedOut -> { model with User = None }
   | SignInButtonClicked -> model
   | SignOutButtonClicked -> model

let setUser = UserSignedIn >> update
let resetUser = UserSignedOut |> update

type private ViewTemplate = Template<const(__SOURCE_DIRECTORY__ + "/Header.html")>

let view model dispatch =
   ViewTemplate()
      .User(
         match model.User with
         | None ->
            ViewTemplate.SignInButton()
               .SignInButtonClicked(fun _ -> SignInButtonClicked |> dispatch)
               .Elt()
         | Some(user) ->
            ViewTemplate.UserInfo()
               .UserName(user.Name)
               .SignOutButtonClicked(fun _ -> SignOutButtonClicked |> dispatch)
               .Elt()
      )
      .Elt()

type UIComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
