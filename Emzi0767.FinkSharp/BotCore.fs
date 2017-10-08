namespace Emzi0767.FinkSharp

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open DSharpPlus
open DSharpPlus.CommandsNext
open DSharpPlus.Entities
open DSharpPlus.EventArgs
open BotLoader

module BotCore =

    let cfg = load_config()

    let dconf = new DiscordConfiguration()
    dconf.set_AutoReconnect true
    dconf.set_LogLevel LogLevel.Debug
    dconf.set_Token cfg.token
    dconf.set_TokenType TokenType.Bot

    let discord = new DiscordClient(dconf)

    let cconf = new CommandsNextConfiguration()
    cconf.set_StringPrefix cfg.prefix
    cconf.set_SelfBot false

    let commands = discord.UseCommandsNext(cconf)
    commands.RegisterCommands<BotCommands>()

    let log_received(s: obj) (e: DebugLogMessageEventArgs) =
        printfn "[%s] [%s] [%s] %s" (e.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")) (e.Level.ToString()) e.Application e.Message

    discord.DebugLogger.LogMessageReceived.AddHandler(EventHandler<DebugLogMessageEventArgs>(log_received))

    let cmde_cbl (e: CommandErrorEventArgs) =
        let embed = new DiscordEmbedBuilder()
        embed.Color <- new DiscordColor(0xFF0000)
        embed.Author <- new DiscordEmbedBuilder.EmbedAuthor()
        embed.Author.Name <- "Error"
        embed.Footer <- new DiscordEmbedBuilder.EmbedFooter()
        embed.Footer.Text <- "BANE"
        embed.Footer.IconUrl <- discord.CurrentUser.AvatarUrl
        embed.Timestamp <- new Nullable<DateTimeOffset>(DateTimeOffset.UtcNow)
        embed.Description <- sprintf "```cs\n%s: %s\n```" (e.Exception.GetType().ToString()) e.Exception.Message

        e.Context.Channel.SendMessageAsync("", false, embed.Build()) :> Task
    
    commands.add_CommandErrored(new AsyncEventHandler<CommandErrorEventArgs>(cmde_cbl))

    let timer_cbl (_: obj) =
        let proc = Process.GetCurrentProcess()
        discord.UpdateStatusAsync(new DiscordGame(sprintf "Up for: %s" ((DateTime.UtcNow - proc.StartTime.ToUniversalTime()).ToString()))) |> Async.AwaitTask |> Async.RunSynchronously

    let timer = new Timer(timer_cbl, null, Timeout.InfiniteTimeSpan, TimeSpan.FromMinutes(1.0))

    let rdy_cbl(ea: ReadyEventArgs) =
        ignore(timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(1.0)))
        Task.CompletedTask

    discord.add_Ready(new AsyncEventHandler<ReadyEventArgs>(rdy_cbl))

    