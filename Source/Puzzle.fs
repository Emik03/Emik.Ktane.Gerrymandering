namespace Emik.Ktane.Gerrymandering

open System
open System.Collections.Generic
open Emik.Morsels.FSharp
open Microsoft.FSharp.Collections

[<NoComparison>]
type Puzzle =
    { Answer : ICollection<(int * int) list>
      Matrix : Hue[,]
      Winner : Hue }

    // Performs 1 warmup round.
    static do
        { Puzzle.Answer = List<_> (); Matrix = Array2D.zeroCreate 6 8; Winner = Blue }
            .Run (Random 2) 3 12 (TimeSpan.FromSeconds 1) |> ignore

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

    member this.Run (rng: Random) (blocLength : int) blocs timeout =
        let start = DateTime.Now
        let { Answer = answer; Matrix = matrix } = this
        answer.Clear ()
        Array.Clear (matrix, 0, matrix.Length)
        let rngFn min max = rng.Next(min, max)
        let mutable appended = List<_> blocLength

        let rec recursive absY absX =
            if (DateTime.Now - start) > timeout then None else

            let isValid (y, x) =
                appended.Count <> blocLength &&
                tryGet y x matrix = Some White &&
                (y, x) |> appended.Contains |> not

            let step (y, x) =
                (y, x) |> appended.Add

                match recursive y x with
                | None -> false
                | Some(false) -> (y, x) |> appended.Remove |> tru
                | Some(true) -> true

            ([ 1, 0; -1, 0; 0, 1; 0, -1 ]
            |> shuffle rngFn
            |> Seq.map (fun (relY, relX) -> (absY + relY, absX + relX))
            |> Seq.where isValid
            |> Seq.forall step).some(appended.Count = blocLength)

        let winners =
            [ for _ in 1 .. (blocs + 2) / 2 -> this.Winner ] @
            [ for _ in 1 .. (blocs - 1) / 2 -> this.Winner.Opposite ]
            |> shuffle rngFn
            |> List.ofSeq

        let mutable limit = blocs
        let mutable hasTime = true

        let push i (y, x) = matrix[y, x] <- winners[limit - 1].OppositeIf <|
                                            (i < (blocLength - 1) / 2 && rngFn 1 4 <> 1)

        while hasTime && limit <> 0 do
            let placedHues =
                matrix
                |> Array2D.mapi tuple3
                |> toSeq
                |> Seq.where (fun (_, _, hue) -> hue <> White)
                |> Seq.map dropRight
                |> tryPickRandom rngFn

            let y, x = placedHues.getOr (fun _ -> pickIndex2 rngFn matrix)

            appended.Clear ()

            match recursive y x with
            | None -> hasTime <- false
            | Some false -> ()
            | Some true -> List.ofSeq appended |> answer.Add
                           appended |> Seq.iteri push
                           limit <- limit - 1

        hasTime
