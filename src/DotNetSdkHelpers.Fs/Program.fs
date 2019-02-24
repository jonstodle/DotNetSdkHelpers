// Learn more about F# at http://fsharp.org

open System
open System.Diagnostics
open Argu

type SDKHelperArguments =
    | [<CliPrefix(CliPrefix.None)>] List
    with
        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | List -> "Lists all installed .NET Core SDKs"
                


let printSDKVersion =
    ProcessStartInfo("dotnet", "--version")
        |> (fun startInfo ->
            startInfo.RedirectStandardOutput <- true
            Process.Start(startInfo))
        |> (fun proc -> proc.StandardOutput.ReadToEnd())
        |> (fun output -> output.Trim())
        |> printfn "%s"
    0
    
[<EntryPoint>]
let main argv =
    let parser = ArgumentParser.Create<SDKHelperArguments>(programName = "dotnet-sdk")
    
    let result = parser.ParseCommandLine argv
    
    result.GetResult()
    
    printfn "Parse result: %A" <| result.GetAllResults()
    0