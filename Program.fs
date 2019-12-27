open System
open System.IO

open Lolcat

module Process =
  let rec processStdInput() =
    match Console.ReadLine() with
    | null -> ()
    | input -> 
        Console.WriteLine (Colorize.colorize input)
        processStdInput()

  let processFile filePath =
    File.ReadAllText filePath
    |> (Colorize.colorize >> Console.WriteLine)

module Help =
  let [<Literal>] HelpMessage = 
    @"Usage example:
    1. dir | lolcat
    2. lolcat some.txt"

  let showHelp() =
    Console.WriteLine (Colorize.colorize HelpMessage)

type Args = 
  private 
    | StdIO
    | File of path : string
    | Help
  with
  static member parseArgs (argv : string array) =
    if Array.isEmpty argv then StdIO
    elif File.Exists argv.[0] then File argv.[0]
    else Help

// Based on ideas https://github.com/gregvinyard/rgbarf
[<EntryPoint>]
let main argv =
  if not (VT100.initMode())
  then 
    failwith "Failed to configure VT100 console mode."
  else 
    match Args.parseArgs argv with
    | File path -> Process.processFile path
    | StdIO when VT100.isInputRedirected() -> Process.processStdInput()
    | _ -> Help.showHelp()

  0
  