using NightmareMode.Attributes;
using NightmareMode.Helpers;
using NightmareMode.Interfaces;
using NightmareMode.Managers;

namespace NightmareMode.Nights;

[RegisterNight]
internal class Night3 : INight
{
    public int Night => 3;

    private string note = "";
    public void InitNight()
    {
        note = NightManager.SummaryNote;
        Utils.SetCallNote(note);
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

    void At_12AM()
    {
        Utils.SetStartTimeAllRandom(5f, 8f);
        AIManager.Toy_FreddyAI?.StartTimer = 15f;
        AIManager.Toy_ChicaAI?.StartTimer = 0f;
        AIManager.MangleAI?.StartTimer = 20f;
        AIManager.PuppetAI?.StartTimer = 0f;
        AIManager.W_BonnieAI?.StartTimer = 0f;

        Utils.SetDifficultyAll(0);
        AIManager.Toy_FreddyAI?.Difficulty = 2;
        AIManager.Toy_BonnieAI?.Difficulty = 4;
        AIManager.Toy_ChicaAI?.Difficulty = 4;
        AIManager.MangleAI?.Difficulty = 25;
        AIManager.PuppetAI?.Difficulty = 7;

        AIManager.W_FreddyAI?.Difficulty = 2;
        AIManager.W_BonnieAI?.Difficulty = 4;
        AIManager.W_ChicaAI?.Difficulty = 4;

        NightManager.DelayedNightAction(() =>
        {
            Utils.InvokeRandomAction(
                () => AIManager.W_BonnieAI?.MoveToPos(2),
                () => AIManager.W_ChicaAI?.MoveToPos(4)
                );
        }, 3f);
    }

    void At_1AM()
    {
        AIManager.Toy_FreddyAI?.Difficulty = 3;
        AIManager.Toy_BonnieAI?.Difficulty = 5;
        AIManager.MangleAI?.Difficulty = 5;
        AIManager.W_FreddyAI?.Difficulty = 3;
        AIManager.W_ChicaAI?.Difficulty = 5;
    }

    void At_2AM()
    {
        AIManager.Toy_FreddyAI?.Difficulty = 0;
        AIManager.BalloonBoyAI?.Difficulty = 3;
        AIManager.W_FoxyAI?.Difficulty = 3;
    }

    void At_3AM()
    {
        AIManager.Toy_FreddyAI?.Difficulty = 6;
        AIManager.Toy_BonnieAI?.Difficulty = 3;
        AIManager.Toy_ChicaAI?.Difficulty = 6;
        AIManager.MangleAI?.Difficulty = 3;
        AIManager.W_FreddyAI?.Difficulty = 5;
        AIManager.W_BonnieAI?.Difficulty = 6;
        AIManager.W_ChicaAI?.Difficulty = 3;
    }

    void At_4AM()
    {
        AIManager.BalloonBoyAI?.Difficulty = 5;
        AIManager.W_FoxyAI?.Difficulty = 5;

        AIManager.Toy_ChicaAI?.TryMoveNextPos();
        AIManager.W_BonnieAI?.TryMoveNextPos();
    }

    private void At_5AM()
    {
        AIManager.Toy_FreddyAI?.Difficulty = 0;
        AIManager.Toy_BonnieAI?.Difficulty = 8;
        AIManager.Toy_ChicaAI?.Difficulty = 6;
        AIManager.MangleAI?.Difficulty = 0;
        AIManager.BalloonBoyAI?.Difficulty = 0;
        AIManager.PuppetAI?.Difficulty = 8;

        AIManager.W_FreddyAI?.Difficulty = 4;
        AIManager.W_BonnieAI?.Difficulty = 6;
        AIManager.W_ChicaAI?.Difficulty = 8;
        AIManager.W_FoxyAI?.Difficulty = 0;

        AIManager.W_FreddyAI?.TryMoveNextPos();
    }

    public void OnWin() { }
}
