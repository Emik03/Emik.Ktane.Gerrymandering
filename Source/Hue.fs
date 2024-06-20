namespace Emik.Ktane.Gerrymandering

[<Struct; StructuredFormatDisplay("{AsString}")>]
type Hue =
    | White
    | Blue
    | Orange

    member this.AsString = this.ToString ()

    member this.Opposite =
        match this with
        | White -> White
        | Blue -> Orange
        | Orange -> Blue

    member this.OppositeIf b = if b then this.Opposite else this

    override this.ToString () =
        match this with
        | White -> " "
        | Blue -> "X"
        | Orange -> "O"

    static member FromBool b = if b then Blue else Orange
