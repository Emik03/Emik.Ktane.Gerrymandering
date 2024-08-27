// SPDX-License-Identifier: MPL-2.0
var sw = Stopwatch.StartNew();

for (var i = 0; i < 5; i++)
    Console.WriteLine(
        new Puzzle([], new Hue[6, 8], Hue.Blue) is var a && a.Run(Rng.Instance, 3, 12, TimeSpan.FromMilliseconds(50))
            ? Cell.ShowMatrix(a.Cells)
            : "Timeout exceeded."
    );

Console.WriteLine(sw.Elapsed);
