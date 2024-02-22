using System;
using System.Threading;

namespace FeatureFilters;

internal static class RandomGenerator
{
    private static readonly Random Global = new();

    private static readonly ThreadLocal<Random> Rnd = new(() =>
    {
        int seed;

        lock (Global)
        {
            seed = Global.Next();
        }

        return new Random(seed);
    });

    public static int Next()
    {
        return Rnd.Value.Next();
    }

    public static double NextDouble()
    {
        return Rnd.Value.NextDouble();
    }
}