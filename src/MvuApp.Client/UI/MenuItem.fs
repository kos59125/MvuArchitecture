module MvuApp.Client.UI.MenuItem

open Bolero

type Model = {
   Url : string
   Text : string
   IsActive : bool
}

type Message =
   | SetActive
   | SetInactive

let init url text =
   {
      Url = url
      Text = text
      IsActive = false
   }

let update message model =
   match message with
   | SetActive -> { model with IsActive = true }
   | SetInactive -> { model with IsActive = false }
   
let setActive = SetActive |> update
let setInactive = SetInactive |> update

type private ViewTemplate = Template<const(__SOURCE_DIRECTORY__ + "/MenuItem.html")>

let view model dispatch =
   ViewTemplate()
      .Url(model.Url)
      .Text(model.Text)
      .Active(
         match model.IsActive with
         | true -> "is-active"
         | false -> ""
      )
      .Elt()

type UIComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
