// SPDX-License-Identifier: MPL-2.0
for (var i = 0; i < 100000; i++)
{
    Puzzle puzzle = new([], new Hue[9, 13], Hue.Blue);
    var seed = Random.Shared.Next();

    if (puzzle.Run(new(seed), 3, 31, TimeSpan.FromMilliseconds(100)) &&
        puzzle.Answer.Select(x => x.Count(x => puzzle.Matrix[x.Item1, x.Item2].IsBlue) >= 2).Count(x => x) <= 15)
    {
        Console.WriteLine(seed);
        Console.WriteLine(Cell.ShowMatrix(puzzle.Cells));
        break;
    }
}
