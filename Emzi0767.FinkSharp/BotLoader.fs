namespace Emzi0767.FinkSharp

open System.IO
open System.Text
open Newtonsoft.Json

module BotLoader =

    type config = {
        token: string
        prefix: string
    }

    let load_config() = 
        let utf8 = new UTF8Encoding(false)
        use fs = File.OpenRead("config.json")
        use sr = new StreamReader(fs, utf8)
        let json = sr.ReadToEnd()

        sr.Dispose()
        fs.Dispose()

        JsonConvert.DeserializeObject<config>(json)
            
            