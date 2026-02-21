using NightmareMode.Data;
using NightmareMode.Enums;
using NightmareMode.Helpers;
using NightmareMode.Interfaces;

namespace NightmareMode.Nights.Challenges;

internal class PresetChallenge : IChallenge
{
    public bool Completed => DataManager.CompletedChallenges.HasCompletedChallenge(ChallengesFlag.None);
    public int ChallengeId => 0;

    private string note = "";
    public void InitChallenge()
    {
        Utils.SetCallNote("");
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
                At_1AM();
                break;
            case 2:
                At_2AM();
                break;
            case 3:
                At_3AM();
                break;
            case 4:
                At_4AM();
                break;
            case 5:
                At_5AM();
                break;
        }
    }

    public void OnHalfHour(int hour) { }

    private void At_12AM() { }

    private void At_1AM() { }

    private void At_2AM() { }

    private void At_3AM() { }

    private void At_4AM() { }

    private void At_5AM() { }

    public void OnWin()
    {
        DataManager.CompletedChallenges.SetChallengeCompleted(ChallengesFlag.None);
    }
}
