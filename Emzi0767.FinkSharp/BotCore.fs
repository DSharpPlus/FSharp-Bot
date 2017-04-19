namespace Emzi0767.FinkSharp

open System
open System.Diagnostics
open System.Linq.Expressions
open System.Threading
open System.Threading.Tasks
open DSharpPlus
open DSharpPlus.Commands
open BotLoader

module BotCore =

    let cfg = load()

    let dconf = new DiscordConfig()
    dconf.AutoReconnect <- true
    dconf.DiscordBranch <- Branch.Stable
    dconf.LogLevel <- LogLevel.Unnecessary
    dconf.Token <- cfg.token
    dconf.TokenType <- TokenType.Bot

    let discord = new DiscordClient(dconf)
    discord.SetSocketImplementation<WebSocketSharpClient>()

    let cconf = new CommandConfig()
    cconf.Prefix <- cfg.prefix
    cconf.SelfBot <- false

    let commands = discord.UseCommands(cconf)

    discord.DebugLogger.LogMessageReceived.AddHandler (fun (s: obj) (e: DebugLogMessageEventArgs) -> printfn "[%s] [%s] [%s] %s" (e.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")) (e.Level.ToString()) e.Application e.Message)

    let cmde_cbl (e: CommandErrorEventArgs) =
        let embed = new DiscordEmbed()
        embed.Color <- 0xFF0000
        embed.Author <- new DiscordEmbedAuthor()
        embed.Author.Name <- "Error"
        embed.Footer <- new DiscordEmbedFooter()
        embed.Footer.Text <- "BANE"
        embed.Footer.IconUrl <- discord.Me.AvatarUrl
        embed.Timestamp <- DateTime.UtcNow
        embed.Description <- sprintf "```cs\n%s: %s\n```" (e.Exception.GetType().ToString()) e.Exception.Message

        e.Channel.SendMessage("", false, embed) |> Async.AwaitTask |> Async.Ignore |> Async.RunSynchronously
        Task.CompletedTask
    commands.add_CommandError(new AsyncEventHandler<CommandErrorEventArgs>(cmde_cbl))

    let timer_cbl (_: obj) =
        let proc = Process.GetCurrentProcess()
        discord.UpdateStatus(sprintf "Up for: %s" ((DateTime.UtcNow - proc.StartTime.ToUniversalTime()).ToString())) |> Async.AwaitTask |> Async.RunSynchronously
    let timer = new Timer(timer_cbl, null, Timeout.InfiniteTimeSpan, TimeSpan.FromMinutes(1.0))

    let rdy_cbl() =
        ignore(timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(1.0)))
        Task.CompletedTask
    discord.add_Ready(new AsyncEventHandler(rdy_cbl))

    for xm in load_cmds() do
        let attr = xm.GetCustomAttributes(false) |> Seq.find(fun xa -> xa.GetType() = typeof<CommandAttribute>) :?> CommandAttribute
        let ea = Expression.Parameter(typeof<CommandEventArgs>, "e")
        let ec = Expression.Call(xm.DeclaringType, xm.Name, null, [| ea :> Expression |])
        let el = Expression.Lambda<Func<CommandEventArgs, Task>>(ec, ea)
        ignore(commands.AddCommand(attr.Name, el.Compile()))