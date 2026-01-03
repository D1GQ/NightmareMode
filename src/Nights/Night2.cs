using NightmareMode.Helpers;
using NightmareMode.Items.Attributes;
using NightmareMode.Items.Interfaces;
using NightmareMode.Managers;

namespace NightmareMode.Nights;

[RegisterNight]
internal class Night2 : INight
{
    public int Night => 2;

    private string note = "";
    public void InitNight()
    {
        note = "";
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

    private void At_12AM()
    {
        note += "I need to keep an eye on Foxy in the hallway tonight! ";
        Utils.SetCallNote(note);

        Utils.SetStartTimeAllRandom(2.5f, 5f);
        AIManager.PuppetAI?.SetStartTime(0f);
        AIManager.W_BonnieAI?.SetStartTime(UnityEngine.Random.Range(5f, 10f));
        AIManager.W_ChicaAI?.SetStartTime(UnityEngine.Random.Range(5f, 10f));
        AIManager.W_FoxyAI?.SetStartTime(10f);

        Utils.SetDifficultyAll(0);
        AIManager.BalloonBoyAI?.SetDifficulty(6);
        AIManager.PuppetAI?.SetDifficulty(6);
        AIManager.W_FreddyAI?.SetDifficulty(6);
        AIManager.W_BonnieAI?.SetDifficulty(3);
        AIManager.W_ChicaAI?.SetDifficulty(4);
        AIManager.W_FoxyAI?.SetDifficulty(3);
    }

    private void At_1AM()
    {
        note += "Balloon Boy isn’t an old friend I’d ever want to meet again. I’ll need to watch both him and Foxy! ";
        Utils.SetCallNote(note);

        AIManager.W_BonnieAI?.SetDifficulty(5);
        AIManager.W_ChicaAI?.SetDifficulty(3);
        AIManager.W_FoxyAI?.SetDifficulty(5);
        AIManager.BalloonBoyAI?.SetDifficulty(3);
    }

    private void At_3AM()
    {
        note += "The older Freddy takes forever to leave the office—almost worse than Mangle, I think. ";
        Utils.SetCallNote(note);

        AIManager.W_FreddyAI?.SetDifficulty(10);
        AIManager.W_FoxyAI?.SetDifficulty(3);
        AIManager.BalloonBoyAI?.SetDifficulty(6);
        AIManager.PuppetAI?.SetDifficulty(7);
    }

    private void At_4AM()
    {
        AIManager.W_ChicaAI?.SetDifficulty(5);

        AIManager.W_BonnieAI?.TryMoveNextPos();
        AIManager.BalloonBoyAI?.TryMoveNextPos();
    }

    private void At_5AM()
    {
        AIManager.W_FreddyAI?.SetDifficulty(6);
        AIManager.W_BonnieAI?.SetDifficulty(6);
        AIManager.W_ChicaAI?.SetDifficulty(6);
        AIManager.W_FoxyAI?.SetDifficulty(5);
        AIManager.BalloonBoyAI?.SetDifficulty(8);
    }

    public void OnWin() { }
}