namespace Lolcat

open System
open System.Runtime.InteropServices

// https://gist.github.com/tomzorz/6142d69852f831fb5393654c90a1f22e
module VT100 =
  let [<Literal>] private ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004u
  let [<Literal>] private DISABLE_NEWLINE_AUTO_RETURN = 0x0008u

  type private FileType = 
    | Unknown = 0 
    | Disk = 1 
    | Char = 2 
    | Pipe = 3

  type private StdHandle =
    | Stdin = -10
    | Stdout = -11
    | Stderr = -12

  module private Imported =
    [<DllImport("kernel32.dll", EntryPoint = "GetConsoleMode")>]
    extern bool tryGetConsoleMode(IntPtr hConsoleHandle, uint32& lpMode)

    [<DllImport("kernel32.dll", EntryPoint = "SetConsoleMode")>]
    extern bool setConsoleMode(IntPtr hConsoleHandle, uint32 dwMode)

    [<DllImport("kernel32.dll", EntryPoint = "GetStdHandle")>]
    extern IntPtr getStdHandle(StdHandle nStdHandle)

    [<DllImport("kernel32.dll", EntryPoint = "GetFileType")>]
    extern FileType getFileType(IntPtr hdl)

    let getConsoleMode handle =
      let mutable mode = 0u
      let success = tryGetConsoleMode(handle, &mode)
      mode, success

  let isOutputRedirected() = FileType.Char <> Imported.getFileType (Imported.getStdHandle StdHandle.Stdout)

  let isInputRedirected() = FileType.Char <> Imported.getFileType (Imported.getStdHandle StdHandle.Stdin)

  let isErrorRedirected() = FileType.Char <> Imported.getFileType (Imported.getStdHandle StdHandle.Stderr)

  let initMode() =
    let stdOut = Imported.getStdHandle StdHandle.Stdout

    match Imported.getConsoleMode(stdOut) with
    | mode, true ->
        let outConsoleMode = mode ||| ENABLE_VIRTUAL_TERMINAL_PROCESSING ||| DISABLE_NEWLINE_AUTO_RETURN
        Imported.setConsoleMode(stdOut, outConsoleMode) 
    | _ -> false
    