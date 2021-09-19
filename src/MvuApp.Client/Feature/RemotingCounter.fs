module MvuApp.Client.Feature.RemotingCounter

open Bolero
open Bolero.Remoting
open Elmish
open MvuApp.Client.UI
open MvuApp.Service.RemoteCounterService

type Model = {
   Counter : Counter.Model
}

type FeatureError =
   | RemoteError of exn

type Message =
   | Increment
   | Increment10
   | Decrement
   | Decrement10
   | GetRemoteResult of int
   | ReceiveCounterMessage of Counter.Message
   | FeatureError of FeatureError

let init () =
   let model = {
      Counter = Counter.init ()
   }
   model, Cmd.none

let update service message model =
   match message with
   | Increment ->
      let currentValue = model.Counter.Input.Value
      let model = { model with Counter = Counter.setBusy model.Counter }
      let cmd = Cmd.OfAsync.either service.IncrementAsync currentValue GetRemoteResult (RemoteError >> FeatureError)
      model, cmd, None
   | Increment10 ->
      let currentValue = model.Counter.Input.Value
      let model = { model with Counter = Counter.setBusy model.Counter }
      let cmd = Cmd.OfAsync.either service.Increment10Async currentValue GetRemoteResult (RemoteError >> FeatureError)
      model, cmd, None
   | Decrement ->
      let currentValue = model.Counter.Input.Value
      let model = { model with Counter = Counter.setBusy model.Counter }
      let cmd = Cmd.OfAsync.either service.DecrementAsync currentValue GetRemoteResult (RemoteError >> FeatureError)
      model, cmd, None
   | Decrement10 ->
      let currentValue = model.Counter.Input.Value
      let model = { model with Counter = Counter.setBusy model.Counter }
      let cmd = Cmd.OfAsync.either service.Decrement10Async currentValue GetRemoteResult (RemoteError >> FeatureError)
      model, cmd, None
   | GetRemoteResult(value) ->
      let model = { model with Counter = (Counter.setValue value >> Counter.setActive) model.Counter }
      model, Cmd.none, None
   | ReceiveCounterMessage(msg) ->
      let model = { model with Counter = Counter.update msg model.Counter }
      let cmd =
         match msg with
         | Counter.IncrementButtonClicked -> Cmd.ofMsg Increment
         | Counter.Increment10ButtonClicked -> Cmd.ofMsg Increment10
         | Counter.DecrementButtonClicked -> Cmd.ofMsg Decrement
         | Counter.Decrement10ButtonClicked -> Cmd.ofMsg Decrement10
         | _ -> Cmd.none
      model, cmd, None
   | FeatureError(error) ->
      let model = { model with Counter = Counter.setActive model.Counter }
      match error with
      | RemoteError(RemoteUnauthorizedException) -> model, Cmd.none, Some(ApplicationFeature.RequestSignIn)
      | RemoteError(ex) -> model, Cmd.none, Some(ApplicationFeature.UnhandledError(ex))

let view model dispatch =
   Html.ecomp<Counter.UIComponent, _, _> [] model.Counter (ReceiveCounterMessage >> dispatch)

type FeatureComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
