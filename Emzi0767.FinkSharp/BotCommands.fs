namespace Emzi0767.FinkSharp

open System
open System.Diagnostics
open System.Threading.Tasks
open System.Security.Cryptography
open DSharpPlus
open DSharpPlus.Commands

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = false)>]
type CommandAttribute(name: string) =
    inherit Attribute()

    member this.Name = name

module BotCommands =

    let async_to_task(work: Async<unit>) =
        Task.Factory.StartNew(fun() -> work |> Async.RunSynchronously)

    let echo(ctx: CommandEventArgs) = 
        let embed = new DiscordEmbed()
        embed.Title <- "🏓 Pong!"

        if ctx.Arguments = null then
            ctx.Message.Respond("E_NOARGS", false, embed) |> Async.AwaitTask |> Async.Ignore
        else
            ctx.Message.Respond(String.Join(" ", ctx.Arguments), false, embed) |> Async.AwaitTask |> Async.Ignore

    [<Command("echo")>]
    let echo_cmd(ctx: CommandEventArgs) = 
        async_to_task(echo(ctx))
    
    let mention_me(ctx: CommandEventArgs) = 
        ctx.Message.Respond(ctx.Author.Mention) |> Async.AwaitTask |> Async.Ignore

    [<Command("mentionme")>]
    let mention_me_cmd(ctx: CommandEventArgs) =
        async_to_task(mention_me(ctx))
    
    let uptime(ctx: CommandEventArgs) =
        let proc = Process.GetCurrentProcess()
        let dt = DateTime.UtcNow
        let uptime = dt - proc.StartTime.ToUniversalTime()
        let rng = new RNGCryptoServiceProvider()
        let clr = Array.zeroCreate<byte> 3
        rng.GetNonZeroBytes(clr)

        let embed = new DiscordEmbed();
        embed.Author <- new DiscordEmbedAuthor();
        embed.Author.Name <- "BANE's uptime"
        embed.Author.IconUrl <- "https://discordapp.com/assets/841d44baf59b5bb6dde668a3d44e8e65.svg"
        embed.Footer <- DiscordEmbedFooter()
        embed.Footer.Text <- "Barely Automated Normie Exterminator"
        embed.Footer.IconUrl <- ctx.Discord.Me.AvatarUrl
        embed.Timestamp <- dt
        embed.Description <- sprintf "%.0f days, %2i hours, %2i minutes, %2i seconds, %3i milliseconds" (Math.Floor(uptime.TotalDays)) uptime.Hours uptime.Minutes uptime.Seconds uptime.Milliseconds
        embed.Color <- (int clr.[0] <<< 16) ||| (int clr.[1] <<< 8) ||| (int clr.[2])

        ctx.Message.Respond("", false, embed) |> Async.AwaitTask |> Async.Ignore

    [<Command("uptime")>]
    let uptime_cmd(ctx: CommandEventArgs) = 
        async_to_task(uptime(ctx))
    
    let colour(ctx: CommandEventArgs) = 
        let dt = DateTime.UtcNow
        let rng = new RNGCryptoServiceProvider()
        let clr = Array.zeroCreate<byte> 3
        rng.GetNonZeroBytes(clr)
        let cli = (int clr.[0] <<< 16) ||| (int clr.[1] <<< 8) ||| (int clr.[2])
        let cls = sprintf "%06x" cli

        let embed = new DiscordEmbed();
        embed.Author <- new DiscordEmbedAuthor();
        embed.Author.Name <- "Colour generator"
        embed.Author.IconUrl <- "https://discordapp.com/assets/35665b6147e6ea2d0a8c6cb759d4a281.svg"
        embed.Footer <- DiscordEmbedFooter()
        embed.Footer.Text <- "Barely Automated Normie Exterminator"
        embed.Footer.IconUrl <- ctx.Discord.Me.AvatarUrl
        embed.Timestamp <- dt
        embed.Description <- sprintf "Your cryptographically-secure colour: [#%s](http://www.colorhexa.com/%s)." cls cls
        embed.Color <- cli

        ctx.Message.Respond("", false, embed) |> Async.AwaitTask |> Async.Ignore
    
    [<Command("colour")>]
    let colour_cmd(ctx: CommandEventArgs) =
        async_to_task(colour(ctx))