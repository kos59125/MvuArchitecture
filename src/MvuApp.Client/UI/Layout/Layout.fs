namespace MvuApp.Client.UI.Layout

open Microsoft.AspNetCore.Components
open Bolero
open Elmish

module Layout1 =

   type Model<'C> = {
      Component : 'C
   }

   type Message<'C, 'M> =
      | SetComponent of 'C
      | ReceiveComponentMessage of 'M

   let init c =
      {
         Component = c
      }

   let update message model =
      match message with
      | SetComponent(c) -> { model with Component = c }
      | ReceiveComponentMessage(_) -> model

   let setComponent<'C, 'M> = Message<'C, 'M>.SetComponent >> update

   let view<'C, 'M> (template:Node -> Node) (viewComponent:'C -> Dispatch<'M> -> Node) (model:Model<'C>) (dispatch:Dispatch<Message<'C, 'M>>) =
      let node = viewComponent model.Component (ReceiveComponentMessage >> dispatch)
      template node

   type LayoutComponent<'C, 'M>() =
      inherit ElmishComponent<Model<'C>, Message<'C, 'M>>()
      [<Parameter>]
      member val Template : Node -> Node = id with get, set
      [<Parameter>]
      member val ViewComponent : 'C -> Dispatch<'M> -> Node = fun _ _ -> Html.empty with get, set
      override this.View model dispatch = view<'C, 'M> this.Template this.ViewComponent model dispatch

   let layout<'L, 'C, 'M, 'A when 'L :> LayoutComponent<'C, 'M>> (template:Node -> Node) (viewComponent:'C -> Dispatch<'M> -> Node) (escalateMessage:'M -> 'A) (model:Model<'C>) (dispatch:Dispatch<'A>) =
      Html.ecomp<'L, _, _> [Attr("Template", template); Attr("ViewComponent", viewComponent)] model (function
         | ReceiveComponentMessage(msg) -> escalateMessage msg |> dispatch
         | _ -> ()
      )

module Layout2 =

   type Model<'C1, 'C2> = {
      Component1 : 'C1
      Component2 : 'C2
   }

   type Message<'C1, 'C2, 'M1, 'M2> =
      | SetComponent1 of 'C1
      | SetComponent2 of 'C2
      | ReceiveComponent1Message of 'M1
      | ReceiveComponent2Message of 'M2

   let init c1 c2 =
      {
         Component1 = c1
         Component2 = c2
      }

   let update message model =
      match message with
      | SetComponent1(c1) -> { model with Component1 = c1 }
      | ReceiveComponent1Message(_) -> model
      | SetComponent2(c2) -> { model with Component2 = c2 }
      | ReceiveComponent2Message(_) -> model

   let setComponent1<'C1, 'C2, 'M1, 'M2> = Message<'C1, 'C2, 'M1, 'M2>.SetComponent1 >> update
   let setComponent2<'C1, 'C2, 'M1, 'M2> = Message<'C1, 'C2, 'M1, 'M2>.SetComponent2 >> update

   let view<'C1, 'C2, 'M1, 'M2>
      (template:Node -> Node -> Node)
      (viewComponent1:'C1 -> Dispatch<'M1> -> Node)
      (viewComponent2:'C2 -> Dispatch<'M2> -> Node)
      (model:Model<'C1, 'C2>)
      (dispatch:Dispatch<Message<'C1, 'C2, 'M1, 'M2>>) =
      let node1 = viewComponent1 model.Component1 (ReceiveComponent1Message >> dispatch)
      let node2 = viewComponent2 model.Component2 (ReceiveComponent2Message >> dispatch)
      template node1 node2

   type LayoutComponent<'C1, 'C2, 'M1, 'M2>() =
      inherit ElmishComponent<Model<'C1, 'C2>, Message<'C1, 'C2, 'M1, 'M2>>()
      [<Parameter>]
      member val Template : Node -> Node -> Node = fun node1 node2 -> Html.concat [node1; node2]  with get, set
      [<Parameter>]
      member val ViewComponent1 : 'C1 -> Dispatch<'M1> -> Node = fun _ _ -> Html.empty with get, set
      [<Parameter>]
      member val ViewComponent2 : 'C2 -> Dispatch<'M2> -> Node = fun _ _ -> Html.empty with get, set
      override this.View model dispatch = view this.Template this.ViewComponent1 this.ViewComponent2 model dispatch

   let layout<'L, 'C1, 'C2, 'M1, 'M2, 'A when 'L :> LayoutComponent<'C1, 'C2, 'M1, 'M2>>
      (template:Node -> Node -> Node)
      (viewComponent1:'C1 -> Dispatch<'M1> -> Node)
      (viewComponent2:'C2 -> Dispatch<'M2> -> Node)
      (escalateMessage1:'M1 -> 'A)
      (escalateMessage2:'M2 -> 'A)
      (model:Model<'C1, 'C2>)
      (dispatch:Dispatch<'A>) =
      Html.ecomp<'L, _, _> [Attr("Template", template); Attr("ViewComponent1", viewComponent1); Attr("ViewComponent2", viewComponent2)] model (function
      | ReceiveComponent1Message(msg) -> escalateMessage1 msg |> dispatch
      | ReceiveComponent2Message(msg) -> escalateMessage2 msg |> dispatch
         | _ -> ()
      )

module Layout3 =

   type Model<'C1, 'C2, 'C3> = {
      Component1 : 'C1
      Component2 : 'C2
      Component3 : 'C3
   }

   type Message<'C1, 'C2, 'C3, 'M1, 'M2, 'M3> =
      | SetComponent1 of 'C1
      | SetComponent2 of 'C2
      | SetComponent3 of 'C3
      | ReceiveComponent1Message of 'M1
      | ReceiveComponent2Message of 'M2
      | ReceiveComponent3Message of 'M3

   let init c1 c2 c3 =
      {
         Component1 = c1
         Component2 = c2
         Component3 = c3
      }

   let update message model =
      match message with
      | SetComponent1(c1) -> { model with Component1 = c1 }
      | ReceiveComponent1Message(_) -> model
      | SetComponent2(c2) -> { model with Component2 = c2 }
      | ReceiveComponent2Message(_) -> model
      | SetComponent3(c3) -> { model with Component3 = c3 }
      | ReceiveComponent3Message(_) -> model
      
   let setComponent1<'C1, 'C2, 'C3, 'M1, 'M2, 'M3> = Message<'C1, 'C2, 'C3, 'M1, 'M2, 'M3>.SetComponent1 >> update
   let setComponent2<'C1, 'C2, 'C3, 'M1, 'M2, 'M3> = Message<'C1, 'C2, 'C3, 'M1, 'M2, 'M3>.SetComponent2 >> update
   let setComponent3<'C1, 'C2, 'C3, 'M1, 'M2, 'M3> = Message<'C1, 'C2, 'C3, 'M1, 'M2, 'M3>.SetComponent3 >> update

   let mapComponent1 mapper model =
      {
         Component1 = mapper model.Component1
         Component2 = model.Component2
         Component3 = model.Component3
      }
   let mapComponent2 mapper model =
      {
         Component1 = model.Component1
         Component2 = mapper model.Component2
         Component3 = model.Component3
      }
   let mapComponent3 mapper model =
      {
         Component1 = model.Component1
         Component2 = model.Component2
         Component3 = mapper model.Component3
      }

   let view<'C1, 'C2, 'C3, 'M1, 'M2, 'M3>
      (template:Node -> Node -> Node -> Node)
      (viewComponent1:'C1 -> Dispatch<'M1> -> Node)
      (viewComponent2:'C2 -> Dispatch<'M2> -> Node)
      (viewComponent3:'C3 -> Dispatch<'M3> -> Node)
      (model:Model<'C1, 'C2, 'C3>)
      (dispatch:Dispatch<Message<'C1, 'C2, 'C3, 'M1, 'M2, 'M3>>) =
      let node1 = viewComponent1 model.Component1 (ReceiveComponent1Message >> dispatch)
      let node2 = viewComponent2 model.Component2 (ReceiveComponent2Message >> dispatch)
      let node3 = viewComponent3 model.Component3 (ReceiveComponent3Message >> dispatch)
      template node1 node2 node3

   type LayoutComponent<'C1, 'C2, 'C3, 'M1, 'M2, 'M3>() =
      inherit ElmishComponent<Model<'C1, 'C2, 'C3>, Message<'C1, 'C2, 'C3, 'M1, 'M2, 'M3>>()
      [<Parameter>]
      member val Template : Node -> Node -> Node -> Node = fun node1 node2 node3 -> Html.concat [node1; node2; node3]  with get, set
      [<Parameter>]
      member val ViewComponent1 : 'C1 -> Dispatch<'M1> -> Node = fun _ _ -> Html.empty with get, set
      [<Parameter>]
      member val ViewComponent2 : 'C2 -> Dispatch<'M2> -> Node = fun _ _ -> Html.empty with get, set
      [<Parameter>]
      member val ViewComponent3 : 'C3 -> Dispatch<'M3> -> Node = fun _ _ -> Html.empty with get, set
      override this.View model dispatch = view this.Template this.ViewComponent1 this.ViewComponent2 this.ViewComponent3 model dispatch
      
   let layout<'L, 'C1, 'C2, 'C3, 'M1, 'M2, 'M3, 'A when 'L :> LayoutComponent<'C1, 'C2, 'C3, 'M1, 'M2, 'M3>>
      (template:Node -> Node -> Node -> Node)
      (viewComponent1:'C1 -> Dispatch<'M1> -> Node)
      (viewComponent2:'C2 -> Dispatch<'M2> -> Node)
      (viewComponent3:'C3 -> Dispatch<'M3> -> Node)
      (escalateMessage1:'M1 -> 'A)
      (escalateMessage2:'M2 -> 'A)
      (escalateMessage3:'M3 -> 'A)
      (model:Model<'C1, 'C2, 'C3>)
      (dispatch:Dispatch<'A>) =
      Html.ecomp<'L, _, _> [Attr("Template", template); Attr("ViewComponent1", viewComponent1); Attr("ViewComponent2", viewComponent2); Attr("ViewComponent3", viewComponent3)] model (function
      | ReceiveComponent1Message(msg) -> escalateMessage1 msg |> dispatch
      | ReceiveComponent2Message(msg) -> escalateMessage2 msg |> dispatch
      | ReceiveComponent3Message(msg) -> escalateMessage3 msg |> dispatch
         | _ -> ()
      )
