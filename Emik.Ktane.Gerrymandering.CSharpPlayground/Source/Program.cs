// SPDX-License-Identifier: MPL-2.0
Puzzle.Warmup();

var sw = Stopwatch.StartNew();

for (var i = 0; i < 100; i++)
    if (new Puzzle([], new Hue[6, 8], Hue.Blue.OppositeIf(i % 2 is 0)) is var puzzle &&
        !puzzle.Run(
            Random.Shared,
            3 + (i / 2 % 2 is 0).ToByte(),
            12 - (i / 2 % 2 is 0).ToByte() * 4,
            TimeSpan.FromMilliseconds(50)
        ))
        throw new TimeoutException();

Console.WriteLine(sw.Elapsed);
