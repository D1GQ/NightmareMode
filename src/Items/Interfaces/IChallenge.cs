namespace NightmareMode.Items.Interfaces;

internal interface IChallenge : ITimeEvent
{
    bool Completed { get; }
    int ChallengeId { get; }
    void InitChallenge();
}
