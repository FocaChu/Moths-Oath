namespace MothsOath.Core.Common;

/// <summary>
/// Centralized random number generator for the game.
/// Thread-safe and uses Random.Shared for better performance.
/// </summary>
public static class GameRandom
{
    /// <summary>
    /// Returns a non-negative random integer.
    /// </summary>
    public static int Next() => Random.Shared.Next();

    /// <summary>
    /// Returns a non-negative random integer that is less than the specified maximum.
    /// </summary>
    /// <param name="maxValue">The exclusive upper bound of the random number to be generated.</param>
    public static int Next(int maxValue) => Random.Shared.Next(maxValue);

    /// <summary>
    /// Returns a random integer that is within a specified range.
    /// </summary>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
    public static int Next(int minValue, int maxValue) => Random.Shared.Next(minValue, maxValue);

    /// <summary>
    /// Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
    /// </summary>
    public static double NextDouble() => Random.Shared.NextDouble();

    /// <summary>
    /// Returns a random boolean value.
    /// </summary>
    public static bool NextBool() => Random.Shared.Next(2) == 1;

    /// <summary>
    /// Returns a random element from the given collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to choose from.</param>
    /// <returns>A random element from the collection.</returns>
    /// <exception cref="ArgumentException">Thrown when the collection is null or empty.</exception>
    public static T GetRandomElement<T>(IList<T> collection)
    {
        if (collection == null || collection.Count == 0)
            throw new ArgumentException("Collection cannot be null or empty.", nameof(collection));

        return collection[Random.Shared.Next(collection.Count)];
    }

    /// <summary>
    /// Shuffles the elements of a list in place using the Fisher-Yates algorithm.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list to shuffle.</param>
    public static void Shuffle<T>(IList<T> list)
    {
        if (list == null || list.Count <= 1)
            return;

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Shared.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    /// <summary>
    /// Performs a percentage check (0-100).
    /// </summary>
    /// <param name="chance">The chance percentage (0-100).</param>
    /// <returns>True if the check succeeds.</returns>
    public static bool RollPercentage(int chance)
    {
        if (chance <= 0) return false;
        if (chance >= 100) return true;
        return Random.Shared.Next(0, 100) < chance;
    }

    /// <summary>
    /// Performs a percentage check with float precision (0.0-1.0).
    /// </summary>
    /// <param name="chance">The chance as a float (0.0-1.0).</param>
    /// <returns>True if the check succeeds.</returns>
    public static bool RollPercentage(float chance)
    {
        if (chance <= 0f) return false;
        if (chance >= 1f) return true;
        return Random.Shared.NextDouble() < chance;
    }
}
