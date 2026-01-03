using NightmareMode.Items.Enums;

namespace NightmareMode.Helpers;

internal static class EnumExtension
{
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
