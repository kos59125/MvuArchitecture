[<AutoOpen>]
module MvuApp.Client.Application.Application

open Elmish
open Bolero
open Bolero.Remoting
open Bolero.Remoting.Client
open Bolero.Templating.Client
open MvuApp.Client.UI.Layout
open MvuApp.Client.Feature
open MvuApp.Client.Screen

type Page =
   | [<EndPoint("/")>] Home
   | [<EndPoint("/counter")>] Counter

type Model = {
   Page : Page
   HeaderScreen : HeaderScreen.Model
   HomeScreen : HomeScreen.Model
   CounterScreen : CounterScreen.Model option
   SignInScreen : SignInScreen.Model option
}

type ApplicationError =
   | AuthenticationFailed of exn
   | UnhandledError of exn

type Message =
   | SetPage of Page
   | GetUser
   | GotUser of AuthenticationService.SignInUserResponseModel
   | StartSignIn
   | SignIn of Credentials
   | SignedIn
   | SignOut
   | SignedOut
   | StartCounter
   | ReceiveHeaderMessage of HeaderScreen.Message
   | ReceiveSignInMessage of SignInScreen.Message
   | ReceiveHomeMessage of HomeScreen.Message
   | ReceiveCounterMessage of CounterScreen.Message
   | ApplicationError of ApplicationError

let init () =
   let header = HeaderScreen.init ()
   let home = HomeScreen.init ()
   let model = {
      Page = Home
      HeaderScreen = header
      HomeScreen = home
      CounterScreen = None
      SignInScreen = None
   }
   let cmd = Cmd.ofMsg GetUser
   model, cmd

let router = Router.infer SetPage (fun model -> model.Page)

let mapAppFeatureCmd = function
   | ApplicationFeatureMessage.RequestSignIn -> Cmd.ofMsg StartSignIn
   | ApplicationFeatureMessage.SignIn(credentials) -> Cmd.ofMsg <| SignIn(credentials)
   | ApplicationFeatureMessage.SignOut -> Cmd.ofMsg SignOut
   | ApplicationFeatureMessage.UnhandledError(errorInfo) -> Cmd.ofMsg <| ApplicationError(UnhandledError(errorInfo))
let mapOptionAppFeatureCmd = function
   | Some(appMsg) -> mapAppFeatureCmd appMsg
   | None -> Cmd.none

let update (program:ProgramComponent<_, _>) message model =
   match message with
   | SetPage(page) ->
      let model = { model with Page = page }
      let cmd =
         match page with
         | Home -> Cmd.none
         | Counter -> Cmd.ofMsg StartCounter
      model, cmd
   | GetUser ->
      let service = program.Remote<AuthenticationService.Service>()
      let cmd = Cmd.OfAsync.perform service.TryGetSignInUserAsync () GotUser
      model, cmd
   | GotUser(user) ->
      let header, appMsg =
         match user with
         | AuthenticationService.NotSignedIn -> HeaderScreen.signedOut model.HeaderScreen
         | AuthenticationService.User(_, name) -> HeaderScreen.signedIn name model.HeaderScreen
      let model = { model with HeaderScreen = header }
      let cmd = mapOptionAppFeatureCmd appMsg
      model, cmd
   | StartSignIn ->
      let modal = SignInScreen.init ()
      let model = { model with SignInScreen = Some(modal) }
      model, Cmd.none
   | SignIn(credentials) ->
      let service = program.Remote<AuthenticationService.Service>()
      let request = { AuthenticationService.SignInRequestModel.Name = credentials.Name; AuthenticationService.SignInRequestModel.Password = credentials.Password }
      let cmd = Cmd.OfAsync.either service.SignInAsync request (fun _ -> SignedIn) (AuthenticationFailed >> ApplicationError)
      model, cmd
   | SignedIn ->
      let model = { model with SignInScreen = None }
      let cmd = Cmd.ofMsg GetUser
      model, cmd
   | SignOut ->
      let service = program.Remote<AuthenticationService.Service>()
      let cmd = Cmd.OfAsync.either service.SignOutAsync () (fun _ -> SignedOut) (AuthenticationFailed >> ApplicationError)
      model, cmd
   | SignedOut ->
      let cmd = Cmd.ofMsg GetUser
      model, cmd
   | StartCounter ->
      let counter, counterCmd = CounterScreen.init ()
      let model = { model with CounterScreen = Some(counter) }
      let cmd = Cmd.map ReceiveCounterMessage counterCmd
      model, cmd
   | ReceiveHeaderMessage(msg) ->
      let header, appMsg = HeaderScreen.update msg model.HeaderScreen
      let model = { model with HeaderScreen = header }
      let cmd = mapOptionAppFeatureCmd appMsg
      model, cmd
   | ReceiveSignInMessage(msg) ->
      match Option.map (SignInScreen.update msg) model.SignInScreen with
      | None -> model, Cmd.none
      | Some(signIn, appMsg) ->
         let model = { model with SignInScreen = Some(signIn) }
         let cmd = mapOptionAppFeatureCmd appMsg
         model, cmd
   | ReceiveHomeMessage(msg) ->
      let model = { model with HomeScreen = HomeScreen.update msg model.HomeScreen }
      model, Cmd.none
   | ReceiveCounterMessage(msg) ->
      let service = program.Remote<RemoteCounterService.Service>()
      match Option.map (CounterScreen.update service msg) model.CounterScreen with
      | None -> model, Cmd.none
      | Some(counter, counterCmd, appMsg) ->
         let model = { model with CounterScreen = Some(counter) }
         let cmd = Cmd.batch [
            Cmd.map ReceiveCounterMessage counterCmd
            mapOptionAppFeatureCmd appMsg
         ]
         model, cmd
   | ApplicationError(error) ->
      match error with
      | AuthenticationFailed(ex) ->
         match model.SignInScreen with
         | None ->
            model, Cmd.none
         | Some(signIn) ->
            let signIn, appMsg = SignInScreen.setSignInError ex.Message signIn
            let model = { model with SignInScreen = Some(signIn) }
            let cmd = mapOptionAppFeatureCmd appMsg
            model, cmd
      | UnhandledError(errorInfo) -> model, Cmd.none  // TODO

let private viewHeaderScreen screen dispatch =
   Html.ecomp<HeaderScreen.ScreenComponent, _, _> [] screen dispatch
let private viewSignInScreen screen dispatch =
   match screen with
   | None -> Html.empty
   | Some(screen) -> Html.ecomp<SignInScreen.ScreenComponent, _, _> [] screen dispatch
let private viewHomeScreen screen dispatch =
   Html.ecomp<HomeScreen.ScreenComponent, _, _> [] screen dispatch
let private viewCounterScreen screen dispatch =
   match screen with
   | None -> Html.empty
   | Some(screen) -> Html.ecomp<CounterScreen.ScreenComponent, _, _> [] screen dispatch

let private layout getMainScreen viewMainScreen receiveMainMessage model dispatch =
   let layoutModel = Layout3.init model.HeaderScreen (getMainScreen model) model.SignInScreen
   Layout3.layout ApplicationLayout.template viewHeaderScreen viewMainScreen viewSignInScreen ReceiveHeaderMessage receiveMainMessage ReceiveSignInMessage layoutModel dispatch

let view model dispatch =
   match model.Page with
   | Home -> layout (fun model -> model.HomeScreen) viewHomeScreen ReceiveHomeMessage model dispatch
   | Counter -> layout (fun model -> model.CounterScreen) viewCounterScreen ReceiveCounterMessage model dispatch

type ApplicationComponent() =
   inherit ProgramComponent<Model, Message>()

   //[<Inject>]
   //member val AuthenticationStateProvider = Unchecked.defaultof<AuthenticationStateProvider> with get, set

   override this.Program =
      Program.mkProgram (fun _ -> init ()) (update this) view
      |> Program.withRouter router
#if DEBUG
      |> Program.withHotReload
#endif
