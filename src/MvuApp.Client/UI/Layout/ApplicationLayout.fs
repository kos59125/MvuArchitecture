module MvuApp.Client.UI.Layout.ApplicationLayout

open Bolero

type private ViewTemplate = Template<const(__SOURCE_DIRECTORY__ + "/ApplicationLayout.html")>

let template (header:Node) (main:Node) (modal:Node) =
   ViewTemplate()
      .Header(header)
      .Main(main)
      .SignInModal(modal)
      .Elt()
