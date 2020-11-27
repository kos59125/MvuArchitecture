module MvuApp.Client.Feature.SignInModal

open Bolero
open MvuApp.Client.UI

type Model = {
   Modal : SignInModal.Model
}

type Message =
   | SetSignInError of string
   | ReceiveUIMessage of SignInModal.Message

let init () =
   {
      Modal = SignInModal.init () |> SignInModal.show
   }

let update message model =
   match message with
   | SetSignInError(errorMessage) ->
      let modal = SignInModal.setError errorMessage model.Modal
      let model = { model with Modal = modal }
      model, None
   | ReceiveUIMessage(msg) ->
      let modal = SignInModal.update msg model.Modal
      let model = { model with Modal = modal }
      let appMsg =
         match msg with
         | SignInModal.SignInButtonClicked ->
            let credentials = { Name = modal.Name; Password = modal.Password }
            Some(ApplicationFeature.SignIn(credentials))
         | _ ->
            None
      model, appMsg

let setSignInError = SetSignInError >> update

let view model dispatch =
   Html.ecomp<SignInModal.UIComponent, _, _> [] model.Modal (ReceiveUIMessage >> dispatch)

type FeatureComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
