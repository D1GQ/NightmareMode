using NightmareMode.Data;
using NightmareMode.Helpers;
using NightmareMode.Items.Attributes;
using NightmareMode.Items.Enums;
using NightmareMode.Items.Interfaces;
using NightmareMode.Managers;

namespace NightmareMode.Nights.Challenges;

[RegisterChallenge]
internal class ChaosShuffleChallenge : IChallenge
{
    public bool Completed => DataManager.CompletedChallenges.HasCompletedChallenge(ChallengesFlag.ChaosShuffleChallenge);
    public int ChallengeId => 3;

    private string note = "";
    public void InitChallenge()
    {
        Utils.SetCallNote("The Animatronics seem a bit quirky tonight... Every hour when the lights flicker all Animatronic levels shift to a random number!");
    }

    public int Hours => 6;
    public void OnHour(int hour)
    {
        switch (hour)
        {
            case 12:
                At_12AM();
                break;
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
                RandomAI();
                break;
        }
    }
    public void OnHalfHour(int hour) { }

    private void At_12AM()
    {
        Utils.SetStartTimeAllRandom(0f, 30f);
        AIManager.PuppetAI?.SetStartTime(0f);

        RandomAI();
    }

    public void OnWin()
    {
        DataManager.CompletedChallenges.SetChallengeCompleted(ChallengesFlag.ChaosShuffleChallenge);
    }

    private void RandomAI()
    {
        Utils.SetDifficultyAllRandom(1, 20);
        AIManager.PuppetAI?.SetDifficulty(10);

        NightManager.PowerSurgeOut();
    }
}
