namespace Emik.Ktane.Gerrymandering

open System
open System.Collections.Generic
open Emik.Morsels.BooleanExtensions
open Emik.Morsels.Courier
open Emik.Morsels.Sequencer
open Emik.Morsels.Randomizer
open Emik.Morsels.OptionExtensions
open Microsoft.FSharp.Collections

[<NoComparison>]
type Puzzle =
    { Answer : ICollection<(int * int) list>
      Matrix : Hue[,]
      Winner : Hue }

    member this.Cells =
        let { Answer = answer; Matrix = matrix } = this
        let windows (l : _ list) = [ 0 .. l.Length - 2 ] |> List.map (fun n -> (l[n], l[n + 1]))
        let cells = matrix |> Array2D.map Cell.FromHue

        let demolishBorder ((yf, xf), (yt, xt)) =
            if (xf = xt) then
                cells[max yf yt, xf].HasTopBorder <- false
            else
                cells[yf, max xf xt].HasLeftBorder <- false

        answer |> Seq.iter (fun x -> windows x |> List.iter demolishBorder)
        cells

    member this.Run rng blocLength blocs timeout =
        let start = DateTime.Now
        let { Answer = answer; Matrix = matrix } = this

        let rec recursive absY absX (appended : ICollection<_>) =
            if (DateTime.Now - start) > timeout then None else

            let isValid (y, x) =
                appended.Count <> blocLength
                && tryGet y x matrix = Some White
                && not (appended.Contains ((y, x)))

            let step (y, x) =
                appended.Add ((y, x))

                match recursive y x appended with
                | None -> false
                | Some(false) -> true
                | Some(true) -> appended.Remove ((y, x)) |> tru

            ([ 1, 0; -1, 0; 0, 1; 0, -1 ]
            |> shuffle rng
            |> Seq.map (fun (relY, relX) -> (absY + relY, absX + relX))
            |> Seq.where isValid
            |> Seq.forall step).some(appended.Count = blocLength)

        let mutable limit = blocs
        let mutable appended = List<_> ()

        let winners =
            [ for _ in 0 .. blocs / 2 -> this.Winner ] @
            [ for _ in (blocs + 3) / 2 .. blocs -> this.Winner.Opposite ]
            |> shuffle rng
            |> List.ofSeq

        let mutable hasTime = true

        while hasTime && limit <> 0 do
            let placedHues =
                matrix
                |> Array2D.mapi tuple3
                |> toSeq
                |> Seq.where (fun (_, _, hue) -> hue <> White)
                |> Seq.map dropRight
                |> tryPickRandom rng

            let y, x = placedHues.getOr (fun _ -> pickIndex2 rng matrix)

            appended.Clear ()

            let push (y, x) = matrix[y, x] <- winners[limit - 1]

            match recursive y x appended with
            | None -> hasTime <- false
            | Some false -> ()
            | Some true -> appended |> List.ofSeq |> answer.Add
                           appended |> shuffle rng |> Seq.iter push
                           limit <- limit - 1

        hasTime
