module MvuApp.Client.Screen.CounterScreen

open Bolero
open Elmish
open MvuApp.Client.UI.Layout
open MvuApp.Client.Feature

type Model = {
   Layout : Layout1.Model<RemotingCounter.Model>
}

type Message =
   | ReceiveCounterMessage of RemotingCounter.Message

let init () =
   let counter, counterCmd = RemotingCounter.init ()
   let model = {
      Layout = Layout1.init counter
   }
   let cmd = Cmd.map ReceiveCounterMessage counterCmd
   model, cmd

let update service message model =
   match message with
   | ReceiveCounterMessage(msg) ->
      let counter, counterCmd, appMsg = RemotingCounter.update service msg model.Layout.Component
      let model = { Layout = Layout1.setComponent counter model.Layout }
      let cmd = Cmd.map ReceiveCounterMessage counterCmd
      model, cmd, appMsg

let view model dispatch =
   let viewComponent = Html.ecomp<RemotingCounter.FeatureComponent, _, _> []
   Layout1.layout CounterScreenLayout.template viewComponent ReceiveCounterMessage model.Layout dispatch

type ScreenComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
