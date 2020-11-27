module MvuApp.Client.UI.ErrorNotification

open Bolero

type Model = {
   Message : string
}

type Message =
   | CloseButtonClicked

let init message =
   {
      Message = message
   }

let update message model =
   match message with
   | CloseButtonClicked -> model

type private ViewTemplate = Template<const(__SOURCE_DIRECTORY__ + "/ErrorNotification.html")>

let view model dispatch =
   ViewTemplate()
      .Message(model.Message)
      .CloseButtonClicked(fun _ -> CloseButtonClicked |> dispatch)
      .Elt()

type UIComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
