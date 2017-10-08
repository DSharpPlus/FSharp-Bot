namespace Emzi0767.FinkSharp

open System.Threading.Tasks
open DSharpPlus
open DSharpPlus.CommandsNext
open DSharpPlus.CommandsNext.Attributes
open DSharpPlus.Entities

type BotCommands() =
    
    member private self.Ping(ctx: CommandContext) = async {
        // trigger a typing indicator
        do! ctx.TriggerTypingAsync() |> Async.AwaitTask

        // create an emoji to make the message a bit more colourful
        let emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:")

        // finally respond with the ping
        do! ctx.RespondAsync(sprintf "%s Pong! Socket latency: %ims" (emoji.ToString()) ctx.Client.Ping) |> Async.AwaitTask |> Async.Ignore
    }

    [<Command("ping"); Description("Responds with socket latency.")>]
    member public self.PingAsync(ctx: CommandContext) = 
        self.Ping(ctx) |> Async.StartAsTask :> Task
    
    member private self.Greet(ctx: CommandContext) (``member``: DiscordMember) = async {
        // trigger a typing indicator
        do! ctx.TriggerTypingAsync() |> Async.AwaitTask

        // create an emoji to make the message a bit more colourful
        let emoji = DiscordEmoji.FromName(ctx.Client, ":wave:")

        // finally respond with the ping
        do! ctx.RespondAsync(sprintf "%s Hello, %s!" (emoji.ToString()) ``member``.Mention) |> Async.AwaitTask |> Async.Ignore
    }

    [<Command("greet"); Description("Says hi to specified user."); Aliases("sayhi", "say_hi")>]
    member public self.GreetAsync(ctx: CommandContext) ([<Description("The user to say hi to.")>] ``member``: DiscordMember) =
        self.Greet ctx ``member`` |> Async.StartAsTask :> Task
