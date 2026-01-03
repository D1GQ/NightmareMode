namespace NightmareMode.Items.Enums;

[Flags]
internal enum NightsFlag
{
    None = 0,
    Night_1 = 1 << 0,
    Night_2 = 1 << 1,
    Night_3 = 1 << 2,
    Night_4 = 1 << 3,
    Night_5 = 1 << 4,
    Night_6 = 1 << 5,
    Night_7 = 1 << 6
}