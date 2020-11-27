module MvuApp.Client.Feature.RemoteCounterService

open Bolero.Remoting

type Service = {
   IncrementAsync : int -> Async<int>
   Increment10Async : int -> Async<int>
   DecrementAsync : int -> Async<int>
   Decrement10Async : int -> Async<int>
} with
   interface IRemoteService with
      member _.BasePath = "/-/counter"
