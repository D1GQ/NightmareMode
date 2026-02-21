namespace NightmareMode.Enums;

/// <summary>
/// Flags enumeration representing the completion status of various challenge modes.
/// Uses bit flags to allow tracking multiple challenge completions in a single integer value.
/// </summary>
[Flags]
internal enum ChallengesFlag
{
    None = 0,
    ToysRevengeChallenge = 1 << 0,
    ChaosShuffleChallenge = 1 << 1,
    OverTimeChallenge = 1 << 2,
    PowerOutage = 1 << 3,
}