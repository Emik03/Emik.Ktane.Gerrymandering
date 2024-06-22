namespace Emik.Ktane.Gerrymandering

open Emik.Morsels.FSharp

type Cell =
    { Hue : Hue
      mutable HasLeftBorder : bool
      mutable HasTopBorder : bool }

    member inline this.LeftBorder = if this.HasLeftBorder then "|" else " "

    member inline this.TopBorder = if this.HasTopBorder then "+---" else "+   "

    static member inline FromHue hue = { Hue = hue; HasLeftBorder = true; HasTopBorder = true }

    static member ShowMatrix (matrix : Cell[,]) =
        let showRow row =
            seq {
                for cell : Cell in row -> cell.TopBorder
                yield "+\n"
                for cell : Cell in row -> cell.LeftBorder + " " + cell.Hue.AsString + " "
                yield "|\n"
            }

        seq {
            yield! matrix |> toJagged |> Seq.collect showRow
            yield String.replicate (matrix.GetLength 1) "+---"
            yield "+"
        } |> String.concat ""
