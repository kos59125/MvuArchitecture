module MvuApp.Client.UI.NumericInput

open Bolero

type Model = {
   Value : int
   IsReadOnly : bool
}

type Message =
   | SetValue of int

let init isReadOnly defaultValue =
   {
      Value = defaultValue
      IsReadOnly = isReadOnly
   }

let update message model =
   match message with
   | SetValue(value) -> { model with Value = value }

let setValue = SetValue >> update

type private ViewTemplate = Template<const(__SOURCE_DIRECTORY__ + "/NumericInput.html")>

let view model dispatch =
   ViewTemplate()
      .Value(model.Value, (SetValue >> dispatch))
      .InputAttr(
         match model.IsReadOnly with
         | true -> [Html.attr.readonly "readonly"]
         | false -> []
      )
      .Elt()

type UIComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
