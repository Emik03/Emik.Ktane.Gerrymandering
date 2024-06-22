namespace Emik.Ktane.Gerrymandering

[<Struct; StructuredFormatDisplay("{AsString}")>]
type Hue =
    | White
    | Blue
    | Orange

    member inline this.AsString = this.ToString ()

    member inline this.Opposite =
        match this with
        | White -> White
        | Blue -> Orange
        | Orange -> Blue

    static member inline FromBool b = if b then Blue else Orange

    member inline this.OppositeIf b = if b then this.Opposite else this

    override this.ToString () =
        match this with
        | White -> " "
        | Blue -> "X"
        | Orange -> "O"
