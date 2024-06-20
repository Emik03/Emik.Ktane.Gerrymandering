open System.Collections.Generic
open Emik.Ktane.Gerrymandering
open System
open Emik.Morsels.Courier
open Emik.Morsels.FunctionWrappers

let go _ =
    let rng = curry (Random ()).Next
    let matrix = Array2D.create 6 8 White
    let ans = List<_> ()
    let puzzle = { Puzzle.Answer = ans; Matrix = matrix; Winner = Blue }

    puzzle.Run rng 3 12

    Cell.ShowMatrix puzzle.Cells |> printfn "%A"

[<EntryPoint>]
let main _ =
    time (fun () -> [ for _ in 1..10 -> go () ]) |> printfn "%A"
    0
