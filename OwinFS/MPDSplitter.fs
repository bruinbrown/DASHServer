module MPDSplitter

open System
open System.IO

//type MPD = FSharp.Data.XmlProvider<>

let OpenFile filename =
    File.OpenRead(filename)

