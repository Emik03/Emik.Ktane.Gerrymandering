namespace Emik.Ktane.Gerrymandering

open System.Collections.Generic
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

    member this.Run rng blocLength blocs =
        let { Answer = answer; Matrix = matrix } = this

        let rec recursive absY absX (appended : ICollection<_>) =
            let isValid (y, x) =
                appended.Count <> blocLength
                && tryGet y x matrix = Some White
                && not (appended.Contains ((y, x)))

            let step (y, x) =
                appended.Add ((y, x))

                if not (recursive y x appended) then
                    appended.Remove ((y, x)) |> ignore

            [ 1, 0; -1, 0; 0, 1; 0, -1 ]
            |> shuffle rng
            |> Seq.map (fun (relY, relX) -> (absY + relY, absX + relX))
            |> Seq.where isValid
            |> Seq.iter step

            appended.Count = blocLength

        let mutable limit = blocs
        let mutable appended = List<_> ()

        let boolRng = toBoolRng rng

        let winners =
            [ for _ in 0 .. limit / 2 -> this.Winner ]
            @ [ for _ in (limit + 1) / 2 .. limit -> this.Winner.Opposite ]
            |> shuffle rng
            |> List.ofSeq

        while limit <> 0 do
            let placedHues =
                matrix
                |> Array2D.mapi tuple3
                |> toSeq
                |> Seq.where (fun (_, _, hue) -> hue <> White)
                |> Seq.map dropRight
                |> tryPickRandom rng

            let y, x = placedHues.getOr (fun _ -> pickIndex2 rng matrix)

            appended.Clear ()

            let odds i = i >= appended.Count / 2 && (boolRng () || boolRng ())

            let push i (y, x) = matrix[y, x] <- winners[limit - 1].OppositeIf <| odds i

            if recursive y x appended then
                appended |> List.ofSeq |> answer.Add
                appended |> shuffle rng |> Seq.iteri push
                limit <- limit - 1
