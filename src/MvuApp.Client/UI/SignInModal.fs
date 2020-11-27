module MvuApp.Client.UI.SignInModal

open Bolero

type Model = {
   Name : string
   Password : string
   IsActive : bool
   ErrorNotification : ErrorNotification.Model option
}

type Message =
   | SetName of string
   | SetPassword of string
   | ShowModal
   | CloseModal
   | SetError of string
   | ClearError
   | ReceiveErrorNotificationMessage of ErrorNotification.Message
   | SignInButtonClicked

let init () =
   {
      Name = ""
      Password = ""
      IsActive = false
      ErrorNotification = None
   }

let update message model =
   match message with
   | SetName(name) -> { model with Name = name }
   | SetPassword(password) -> { model with Password = password }
   | ShowModal -> { model with IsActive = true }
   | CloseModal -> { model with IsActive = false }
   | SetError(message) -> { model with ErrorNotification = Some(ErrorNotification.init message) }
   | ClearError -> { model with ErrorNotification = None }
   | ReceiveErrorNotificationMessage(ErrorNotification.CloseButtonClicked) -> { model with ErrorNotification = None }
   | SignInButtonClicked -> model

let show = ShowModal |> update
let close = CloseModal |> update
let clear = update (SetName("")) >> update (SetPassword(""))
let setError = SetError >> update

type private ViewTemplate = Template<const(__SOURCE_DIRECTORY__ + "/SignInModal.html")>

let view model dispatch =
   ViewTemplate()
      .Name(model.Name, fun value -> SetName(value) |> dispatch)
      .Password(model.Password, fun value -> SetPassword(value) |> dispatch)
      .ModalActive(
         match model.IsActive with
         | true -> [Html.attr.classes ["is-active"]]
         | false -> []
      )
      .SignInButtonClicked(fun _ -> SignInButtonClicked |> dispatch)
      .CloseButtonClicked(fun _ -> CloseModal |> dispatch)
      .ErrorNotification(
         match model.ErrorNotification with
         | None -> Html.empty
         | Some(error) -> Html.ecomp<ErrorNotification.UIComponent, _, _> [] error (ReceiveErrorNotificationMessage >> dispatch)
      )
      .Elt()

type UIComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
