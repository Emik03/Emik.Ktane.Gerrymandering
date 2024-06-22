open System
open System.Collections.Generic
open Emik.Ktane.Gerrymandering
open Emik.Morsels.FSharp

let go _ =
    let rng = curry (Random ()).Next
    let matrix = Array2D.zeroCreate 6 8
    let ans = List<_> ()
    let puzzle = { Puzzle.Answer = ans; Matrix = matrix; Winner = Blue }
    let timeout = TimeSpan.FromMilliseconds 50

    if puzzle.Run rng 3 12 timeout then Cell.ShowMatrix puzzle.Cells |> printfn "%A"
    else printfn "Timeout exceeded."

[<EntryPoint>]
let main _ =
    let inline fn _ = [ for _ in 1 .. 1000 -> go () ]
    time fn |> printfn "%A"
    0
