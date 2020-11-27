[<AutoOpen>]
module MvuApp.Client.Feature.ApplicationFeature

type Credentials = {
   Name : string
   Password : string
}

type ApplicationFeatureMessage =
   | RequestSignIn
   | SignIn of Credentials
   | SignOut
   | UnhandledError of exn
