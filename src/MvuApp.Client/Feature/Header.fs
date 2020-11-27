module MvuApp.Client.Feature.Header

open Bolero
open MvuApp.Client.UI

type Model = {
   Header : Header.Model
}

type Message =
   | SignedIn of string
   | SignedOut
   | ReceiveUIMessage of Header.Message
   
let init () =
   {
      Header = Header.init ()
   }

let update message model =
   match message with
   | SignedIn(userName) ->
      let header = Header.setUser { Header.Name = userName } model.Header
      let model = { Header = header }
      model, None
   | SignedOut ->
      let header = Header.resetUser model.Header
      let model = { Header = header }
      model, None
   | ReceiveUIMessage(msg) ->
      let header = Header.update msg model.Header
      let model = { model with Header = header }
      let appMsg =
         match msg with
         | Header.SignInButtonClicked -> Some(ApplicationFeature.RequestSignIn)
         | Header.SignOutButtonClicked -> Some(ApplicationFeature.SignOut)
         | _ -> None
      model, appMsg

let signedIn = SignedIn >> update
let signedOut = SignedOut |> update

let view model dispatch =
   Html.ecomp<Header.UIComponent, _, _> [] model.Header (ReceiveUIMessage >> dispatch)

type FeatureComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
