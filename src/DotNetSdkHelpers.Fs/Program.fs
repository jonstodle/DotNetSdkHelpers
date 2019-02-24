// Learn more about F# at http://fsharp.org

open System
open System.Diagnostics
open Argu

[<EntryPoint>]
let main argv =
    ProcessStartInfo("dotnet", "--version")
        |> (fun startInfo ->
            startInfo.RedirectStandardOutput <- true
            Process.Start(startInfo))
        |> (fun proc -> proc.StandardOutput.ReadToEnd())
        |> (fun output -> output.Trim())
        |> printfn "%s"
    0

