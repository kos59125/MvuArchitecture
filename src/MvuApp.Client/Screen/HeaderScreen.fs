module MvuApp.Client.Screen.HeaderScreen

open Bolero
open MvuApp.Client.Feature

type Model = {
   Header : Header.Model
}

type Message =
   | SignedIn of string
   | SignedOut
   | ReceiveFeatureMessage of Header.Message

let init () =
   {
      Header = Header.init ()
   }

let update message model =
   match message with
   | SignedIn(userName) ->
      let header, appMsg = Header.signedIn userName model.Header
      let model = { model with Header = header }
      model, appMsg
   | SignedOut ->
      let header, appMsg = Header.signedOut model.Header
      let model = { model with Header = header }
      model, appMsg
   | ReceiveFeatureMessage(msg) ->
      let header, appMsg = Header.update msg model.Header
      let model = { model with Header = header }
      model, appMsg

let signedIn = SignedIn >> update
let signedOut = SignedOut |> update

let view model dispatch =
   Html.ecomp<Header.FeatureComponent, _, _> [] model.Header (ReceiveFeatureMessage >> dispatch)

type ScreenComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
