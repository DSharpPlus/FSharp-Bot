// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System.Threading.Tasks
open Emzi0767.FinkSharp.BotCore

[<EntryPoint>]
let main argv = 
    discord.ConnectAsync() |> Async.AwaitTask |> Async.RunSynchronously
    Task.Delay(-1) |> Async.AwaitTask |> Async.RunSynchronously
    0 // return an integer exit code
