namespace MvuApp.Server

open System

type User = {
   UserId : Guid
   Name : string
   Password : string
}

type IUserRepository =
   abstract TryGetUser : string -> string -> User option

type UserRepository() =

   let store = [
      { UserId = Guid.NewGuid(); Name = "user"; Password = "password" }
   ]

   interface IUserRepository with
      member _.TryGetUser name password =
         List.tryFind (fun user -> user.Name = name && user.Password = password) store
