namespace NightmareMode.Helpers;

/// <summary>
/// Provides extension methods for shuffling collections using the Fisher-Yates algorithm.
/// Enables random reordering of any IEnumerable collection.
/// </summary>
internal static class ShuffleListExtension
{
    private static readonly Random random = new();

    /// <summary>
    /// Randomly shuffles the elements of a collection using the Fisher-Yates algorithm.
    /// Creates a new list with the elements in random order; does not modify the original collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to shuffle. If null, an empty collection will be returned.</param>
    /// <returns>A new IEnumerable containing the same elements in random order.</returns>
    internal static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
    {
        var list = collection.ToList();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
        return list;
    }
}