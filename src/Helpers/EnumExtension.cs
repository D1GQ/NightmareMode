using NightmareMode.Enums;

namespace NightmareMode.Helpers;

/// <summary>
/// Provides extension methods for working with game-specific enumerations.
/// Simplifies common conversions and operations on enum types.
/// </summary>
internal static class EnumExtension
{
    /// <summary>
    /// Converts a night number to its corresponding NightsFlag enum value.
    /// Uses bit shifting to create the appropriate flag based on the night number.
    /// </summary>
    /// <param name="nightNumber">The night number to convert (1-7).</param>
    /// <returns>The NightsFlag value representing the specified night.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when nightNumber is less than 1 or greater than the maximum night value (7).
    /// </exception>
    internal static NightsFlag ToNightFlag(this int nightNumber)
    {
        var allFlags = Enum.GetValues(typeof(NightsFlag))
                          .Cast<NightsFlag>()
                          .ToArray();

        int maxNight = allFlags.Length - 1;

        if (nightNumber < 1 || nightNumber > maxNight)
            throw new ArgumentOutOfRangeException(
                nameof(nightNumber),
                $"Night number must be between 1-{maxNight}");

        return (NightsFlag)(1 << (nightNumber - 1));
    }
}