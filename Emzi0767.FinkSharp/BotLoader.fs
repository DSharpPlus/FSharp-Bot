namespace Emzi0767.FinkSharp

open System.IO
open System.Reflection
open System.Text
open Microsoft.FSharp.Reflection
open Newtonsoft.Json

module BotLoader =

    type config = {
        token: string
        prefix: string
    }

    let load() = 
        let utf8 = new UTF8Encoding(false)
        use fs = File.OpenRead("config.json")
        use sr = new StreamReader(fs, utf8)
        let json = sr.ReadToEnd()

        sr.Dispose()
        fs.Dispose()

        JsonConvert.DeserializeObject<config>(json)
    
    let load_cmds() = 
        let a = Assembly.GetExecutingAssembly()
        let m = Array.find (fun xm -> FSharpType.IsModule xm && xm.Name = "BotCommands") (a.GetTypes())
        if m = null then
            failwith "Module not found"
            
        m.GetMethods() |> Seq.filter (fun xm -> xm.CustomAttributes |> Seq.exists (fun xa -> xa.AttributeType = typeof<CommandAttribute>))
            
            