namespace MvuApp.Server

open Bolero.Remoting.Server
open MvuApp.Client.Feature.RemoteCounterService

type CounterService(remote:IRemoteContext) =
   inherit RemoteHandler<Service>()

   override _.Handler = {
      IncrementAsync = fun x -> async {
         do! Async.Sleep(abs x * 100)
         return x + 1
      }

      Increment10Async = remote.Authorize <| fun x -> async {
         do! Async.Sleep(abs x * 10)
         return x + 10
      }

      DecrementAsync = fun x -> async {
         do! Async.Sleep(abs x * 100)
         return x - 1
      }

      Decrement10Async = remote.Authorize <| fun x -> async {
         do! Async.Sleep(abs x * 10)
         return x - 10
      }
   }
