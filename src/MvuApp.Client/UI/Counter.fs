module MvuApp.Client.UI.Counter

open Bolero

type Model = {
   Input : NumericInput.Model
   IncrementButton : Button.Model
   Increment10Button : Button.Model
   DecrementButton : Button.Model
   Decrement10Button : Button.Model
}

type Message =
   | SetValue of int
   | SetButtonActive
   | SetButtonBusy
   | IncrementButtonClicked
   | Increment10ButtonClicked
   | DecrementButtonClicked
   | Decrement10ButtonClicked

let init () =
   {
      Input = NumericInput.init true 0
      IncrementButton = Button.init "+1"
      Increment10Button = Button.init "+10"
      DecrementButton = Button.init "-1"
      Decrement10Button = Button.init "-10"
   }

let update message model =
   match message with
   | SetValue(value) -> { model with Input = NumericInput.setValue value model.Input }
   | SetButtonActive -> { model with IncrementButton = Button.setActive model.IncrementButton; Increment10Button = Button.setActive model.Increment10Button; DecrementButton = Button.setActive model.DecrementButton; Decrement10Button = Button.setActive model.Decrement10Button }
   | SetButtonBusy -> { model with IncrementButton = Button.setBusy model.IncrementButton; Increment10Button = Button.setBusy model.Increment10Button; DecrementButton = Button.setBusy model.DecrementButton; Decrement10Button = Button.setBusy model.Decrement10Button }
   | IncrementButtonClicked -> model
   | Increment10ButtonClicked -> model
   | DecrementButtonClicked -> model
   | Decrement10ButtonClicked -> model

let setValue = SetValue >> update
let setBusy = SetButtonBusy |> update
let setActive = SetButtonActive |> update

type private ViewTemplate = Template<const(__SOURCE_DIRECTORY__ + "/Counter.html")>

let view model dispatch =
   ViewTemplate()
      .IncrementButton(Html.ecomp<Button.UIComponent, _, _> [] model.IncrementButton (function
         | Button.Clicked -> IncrementButtonClicked |> dispatch
         | _ -> ()
      ))
      .Increment10Button(Html.ecomp<Button.UIComponent, _, _> [] model.Increment10Button (function
         | Button.Clicked -> Increment10ButtonClicked |> dispatch
         | _ -> ()
      ))
      .DecrementButton(Html.ecomp<Button.UIComponent, _, _> [] model.DecrementButton (function
         | Button.Clicked -> DecrementButtonClicked |> dispatch
         | _ -> ()
      ))
      .Decrement10Button(Html.ecomp<Button.UIComponent, _, _> [] model.Decrement10Button (function
         | Button.Clicked -> Decrement10ButtonClicked |> dispatch
         | _ -> ()
      ))
      .Count(Html.ecomp<NumericInput.UIComponent, _, _> [] model.Input ignore)
      .Elt()

type UIComponent() =
   inherit ElmishComponent<Model, Message>()
   override _.View model dispatch = view model dispatch
