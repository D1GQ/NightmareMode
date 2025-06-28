using NightmareMode.Helpers;
using NightmareMode.Items.Attributes;
using NightmareMode.Items.Interfaces;
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
        AIManager.Toy_FreddyAI?.SetStartTime(15f);
        AIManager.Toy_ChicaAI?.SetStartTime(0f);
        AIManager.MangleAI?.SetStartTime(20f);
        AIManager.PuppetAI?.SetStartTime(0f);
        AIManager.W_BonnieAI?.SetStartTime(0f);

        Utils.SetDifficultyAll(0);
        AIManager.Toy_FreddyAI?.SetDifficulty(2);
        AIManager.Toy_BonnieAI?.SetDifficulty(4);
        AIManager.Toy_ChicaAI?.SetDifficulty(4);
        AIManager.MangleAI?.SetDifficulty(25);
        AIManager.PuppetAI?.SetDifficulty(7);

        AIManager.W_FreddyAI?.SetDifficulty(2);
        AIManager.W_BonnieAI?.SetDifficulty(4);
        AIManager.W_ChicaAI?.SetDifficulty(4);

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
        AIManager.Toy_FreddyAI?.SetDifficulty(3);
        AIManager.Toy_BonnieAI?.SetDifficulty(5);
        AIManager.MangleAI?.SetDifficulty(5);
        AIManager.W_FreddyAI?.SetDifficulty(3);
        AIManager.W_ChicaAI?.SetDifficulty(5);
    }

    void At_2AM()
    {
        AIManager.Toy_FreddyAI?.SetDifficulty(0);
        AIManager.BalloonBoyAI?.SetDifficulty(3);
        AIManager.W_FoxyAI?.SetDifficulty(3);
    }

    void At_3AM()
    {
        AIManager.Toy_FreddyAI?.SetDifficulty(6);
        AIManager.Toy_BonnieAI?.SetDifficulty(3);
        AIManager.Toy_ChicaAI?.SetDifficulty(6);
        AIManager.MangleAI?.SetDifficulty(3);
        AIManager.W_FreddyAI?.SetDifficulty(5);
        AIManager.W_BonnieAI?.SetDifficulty(6);
        AIManager.W_ChicaAI?.SetDifficulty(3);
    }

    void At_4AM()
    {
        AIManager.BalloonBoyAI?.SetDifficulty(5);
        AIManager.W_FoxyAI?.SetDifficulty(5);

        AIManager.Toy_ChicaAI?.TryMoveNextPos();
        AIManager.W_BonnieAI?.TryMoveNextPos();
    }

    private void At_5AM()
    {
        AIManager.Toy_FreddyAI?.SetDifficulty(0);
        AIManager.Toy_BonnieAI?.SetDifficulty(8);
        AIManager.Toy_ChicaAI?.SetDifficulty(6);
        AIManager.MangleAI?.SetDifficulty(0);
        AIManager.BalloonBoyAI?.SetDifficulty(0);
        AIManager.PuppetAI?.SetDifficulty(8);

        AIManager.W_FreddyAI?.SetDifficulty(4);
        AIManager.W_BonnieAI?.SetDifficulty(6);
        AIManager.W_ChicaAI?.SetDifficulty(8);
        AIManager.W_FoxyAI?.SetDifficulty(0);

        AIManager.W_FreddyAI?.TryMoveNextPos();
    }

    public void OnWin() { }
}
