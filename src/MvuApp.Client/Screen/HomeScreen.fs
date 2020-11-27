module MvuApp.Client.Screen.HomeScreen

open Bolero
open Elmish
open MvuApp.Client.UI.Layout

type Model = {
   Layout : Layout1.Model<unit>
}

type Message = unit

let init () =
   {
      Layout = Layout1.init ()
   }

let update () model = model

let view model dispatch =
   let viewComponent () (dispatch:Dispatch<Message>) = Html.empty
   Layout1.layout HomeScreenLayout.template viewComponent ignore model.Layout dispatch

type ScreenComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
