open Owin
open Microsoft.Owin
open Microsoft.Owin.Hosting
open System.Text.RegularExpressions

type StreamType = 
    | Audio
    | Video

let Videos = [ (1, """movie.mpd""") ] |> dict

let GetVideoFromDictionary id =
    Videos.[id]

let GetInitializationSegment videoId streamType = 
    match streamType with
    | Audio -> [| 88uy |]
    | Video -> [| 87uy |]

let GetSegment videoId segmentId streamType = 
    match streamType with
    | Audio -> [| 86uy |]
    | Video -> [| 85uy |]

let GetStreamType str = 
    if str = "video" then Video
    else Audio

let HandleRequest (req : Types.OwinRequest) (res : Types.OwinResponse) = 
    let pathsegments = req.Path.TrimStart('/').Split('/')
    let streamType = GetStreamType pathsegments.[0]
    let pathparts = pathsegments.[1].TrimStart('/').Split('-') |> Array.map int
    
    let toWrite = 
        if pathparts.Length = 1 then 
            GetInitializationSegment pathparts.[0] streamType
        else GetSegment pathparts.[0] pathparts.[1] streamType
    do res.SetHeader("Content-Type", "text/plain") |> ignore
    res.WriteAsync(toWrite)

let HandleFailure (req : Types.OwinRequest) (res : Types.OwinResponse) = 
    res.Set(Types.OwinConstants.ResponseStatusCode, 404) |> ignore
    res.WriteAsync("404: Content not found")

let RequestHandler (req : Types.OwinRequest) (res : Types.OwinResponse) = 
    (* URL should be in the form /:video(-:segment)?/ *)
    if Regex.IsMatch(req.Path, """/(audio|video)/[0-9]+(-[0-9]+)?""") then 
        HandleRequest req res
    else HandleFailure req res

type Startup() = 
    member this.Configuration(app : IAppBuilder) = 
        app.UseHandlerAsync(RequestHandler) |> ignore

[<EntryPoint>]
let main argv = 
    use app = WebApp.Start<Startup>("http://localhost:12345")
    System.Console.ReadLine() |> ignore
    0