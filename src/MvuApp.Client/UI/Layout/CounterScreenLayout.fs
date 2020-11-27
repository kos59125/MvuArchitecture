module MvuApp.Client.UI.Layout.CounterScreenLayout

open Bolero

type private ViewTemplate = Template<const(__SOURCE_DIRECTORY__ + "/CounterScreenLayout.html")>

let template (node:Node) =
   ViewTemplate()
      .Counter(node)
      .Elt()
