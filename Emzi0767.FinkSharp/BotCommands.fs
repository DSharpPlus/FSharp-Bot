namespace Emzi0767.FinkSharp

open DSharpPlus
open DSharpPlus.CommandsNext
open DSharpPlus.CommandsNext.Attributes
open DSharpPlus.Entities

type BotCommands() =
    
    let Ping(ctx: CommandContext) = async {
        let emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:")
        do! ctx.RespondAsync(sprintf "%s Pong! Socket latency: %i" (emoji.ToString()) ctx.Client.Ping) |> Async.AwaitTask |> Async.Ignore
    }

    [<Command("ping"); Description("Responds with socket latency.")>]
    let PingAsync(ctx: CommandContext) = 
        Ping(ctx) |> Async.StartAsTask