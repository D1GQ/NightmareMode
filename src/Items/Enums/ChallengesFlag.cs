namespace NightmareMode.Items.Enums;

[Flags]
internal enum ChallengesFlag
{
    None = 0,
    ToysRevengeChallenge = 1 << 0,
    ChaosShuffleChallenge = 1 << 1,
    OverTimeChallenge = 1 << 2,
    PowerOutage = 1 << 3,
}