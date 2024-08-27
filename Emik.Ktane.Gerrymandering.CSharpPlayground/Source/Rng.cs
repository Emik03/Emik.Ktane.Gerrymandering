// SPDX-License-Identifier: MPL-2.0
namespace Emik.Ktane.Gerrymandering.CSharpPlayground;

sealed class Rng : OptimizedClosures.FSharpFunc<int, int, int>
{
    Rng() { }

    public static Rng Instance { get; } = new();

    /// <inheritdoc />
    public override int Invoke(int arg1, int arg2) => Random.Shared.Next(arg1, arg2);
}
