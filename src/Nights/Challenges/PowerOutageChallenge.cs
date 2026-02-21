using NightmareMode.Attributes;
using NightmareMode.Data;
using NightmareMode.Enums;
using NightmareMode.Helpers;
using NightmareMode.Interfaces;
using NightmareMode.Managers;
using NightmareMode.Modules;

namespace NightmareMode.Nights.Challenges;

[RegisterChallenge]
internal class PowerOutageChallenge : IChallenge
{
    public bool Completed => DataManager.CompletedChallenges.HasCompletedChallenge(ChallengesFlag.PowerOutage);
    public int ChallengeId => 2;

    private string note = "";
    public void InitChallenge()
    {
        Utils.SetCallNote(Translator.Get("Note.PowerOutage"));
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

        if (hour >= 6 && hour < 12) return;

        NightManager.PowerSurgeOut();
    }

    public void OnHalfHour(int hour)
    {
        if (hour >= 6 && hour < 12) return;

        OutBreakers();

        AIManager.W_BonnieAI?.GetActivePose()?.Timer += 10f;
        AIManager.W_ChicaAI?.GetActivePose()?.Timer += 10f;
    }

    private void At_12AM()
    {
        Utils.SetStartTimeAllRandom(0f, 10f);
        AIManager.PuppetAI?.StartTimer = 0f;

        Utils.SetDifficultyAll(0);
        AIManager.BalloonBoyAI?.Difficulty = 4;
        AIManager.PuppetAI?.Difficulty = 8;
        AIManager.W_FreddyAI?.Difficulty = 4;
        AIManager.W_BonnieAI?.Difficulty = 6;
        AIManager.W_ChicaAI?.Difficulty = 6;
        AIManager.W_FoxyAI?.Difficulty = 8;
    }

    private void At_1AM()
    {
        AIManager.W_FreddyAI?.Difficulty = 6;
        Utils.InvokeRandomAction(() =>
        {
            AIManager.W_BonnieAI?.Difficulty = 8;
            AIManager.W_ChicaAI?.Difficulty = 4;
        }, () =>
        {
            AIManager.W_BonnieAI?.Difficulty = 4;
            AIManager.W_ChicaAI?.Difficulty = 8;
        });
    }

    private void At_2AM()
    {
        AIManager.W_FreddyAI?.Difficulty = 2;
        AIManager.W_BonnieAI?.Difficulty = 7;
        AIManager.W_ChicaAI?.Difficulty = 7;
    }

    private void At_3AM()
    {
        AIManager.W_FreddyAI?.Difficulty = 6;
        AIManager.W_BonnieAI?.Difficulty = 4;
        AIManager.W_ChicaAI?.Difficulty = 4;
    }
    private void At_4AM() { }

    private void At_5AM()
    {
        AIManager.W_FreddyAI?.Difficulty = 6;
        AIManager.W_BonnieAI?.Difficulty = 6;
        AIManager.W_ChicaAI?.Difficulty = 6;
    }

    private void OutBreakers()
    {
        BreakerRandomSwitches(AIManager.Toy_FreddyAI?.AI?.choice1?.Breaker);
        BreakerRandomSwitches(AIManager.Toy_FreddyAI?.AI?.choice2?.Breaker);
        BreakerRandomSwitches(AIManager.Toy_FreddyAI?.AI?.choice3?.Breaker);
        BreakerRandomSwitches(AIManager.Toy_FreddyAI?.AI?.choice4?.Breaker);
    }

    private void BreakerRandomSwitches(BreakerScript? breaker)
    {
        if (breaker == null) return;

        int switches = 0;

        var switchesListRandom = breaker.switcheslist.Shuffle();

        foreach (var @switch in switchesListRandom)
        {
            if (@switch == null || !@switch.on) continue;

            if (UnityEngine.Random.value < 0.2f) continue;

            if (UnityEngine.Random.value > (0.5f + (0.2f * switches)) || switches == 0)
            {
                @switch.ForceOff();
                switches++;
            }
        }
    }

    public void OnWin()
    {
        DataManager.CompletedChallenges.SetChallengeCompleted(ChallengesFlag.PowerOutage);
    }
}
