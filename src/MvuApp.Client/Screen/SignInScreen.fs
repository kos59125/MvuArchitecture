module MvuApp.Client.Screen.SignInScreen

open Bolero
open MvuApp.Client.Feature

type Model = {
   Modal : SignInModal.Model
}

type Message =
   | SetSignInError of string
   | ReceiveFeatureMessage of SignInModal.Message

let init () =
   {
      Modal = SignInModal.init ()
   }

let update message model =
   match message with
   | SetSignInError(errorMessage) ->
      let modal, appMsg = SignInModal.setSignInError errorMessage model.Modal
      let model = { model with Modal = modal }
      model, appMsg
   | ReceiveFeatureMessage(msg) ->
      let modal, appMsg = SignInModal.update msg model.Modal
      let model = { model with Modal = modal }
      model, appMsg

let setSignInError = SetSignInError >> update

let view model dispatch =
   Html.ecomp<SignInModal.FeatureComponent, _, _> [] model.Modal (ReceiveFeatureMessage >> dispatch)

type ScreenComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
