module MvuApp.Client.UI.Button

open Bolero

type ButtonState =
   | Active
   | Disabled
   | Busy

type Model = {
   Text : string
   State : ButtonState
}

type Message =
   | SetText of string
   | SetState of ButtonState
   | Clicked

let init text =
   {
      Text = text
      State = Active
   }

let update message model =
   match message with
   | SetText(text) -> { model with Text = text }
   | SetState(state) -> { model with State = state }
   | Clicked -> model
   
let setText = SetText >> update
let setActive = SetState(Active) |> update
let setDisabled = SetState(Disabled) |> update
let setBusy = SetState(Busy) |> update

type private ViewTemplate = Template<const(__SOURCE_DIRECTORY__ + "/Button.html")>

let view model dispatch =
   ViewTemplate()
      .ButtonAttr(
         match model.State with
         | Active -> []
         | Disabled -> [Html.attr.disabled "disabled"]
         | Busy -> [Html.attr.disabled "disabled"; Html.attr.classes ["is-loading"]]
      )
      .Text(model.Text)
      .Clicked(fun _ -> Clicked |> dispatch)
      .Elt()

type UIComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
