using NightmareMode.Attributes;
using NightmareMode.Helpers;
using NightmareMode.Interfaces;
using NightmareMode.Managers;
using NightmareMode.Modules;

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
        note += Translator.Get("Note.Night2.Part1") + " ";
        Utils.SetCallNote(note);

        Utils.SetStartTimeAllRandom(2.5f, 5f);
        AIManager.PuppetAI?.StartTimer = 0f;
        AIManager.W_BonnieAI?.StartTimer = UnityEngine.Random.Range(5f, 10f);
        AIManager.W_ChicaAI?.StartTimer = UnityEngine.Random.Range(5f, 10f);
        AIManager.W_FoxyAI?.StartTimer = 10f;

        Utils.SetDifficultyAll(0);
        AIManager.BalloonBoyAI?.Difficulty = 6;
        AIManager.PuppetAI?.Difficulty = 6;
        AIManager.W_FreddyAI?.Difficulty = 6;
        AIManager.W_BonnieAI?.Difficulty = 3;
        AIManager.W_ChicaAI?.Difficulty = 4;
        AIManager.W_FoxyAI?.Difficulty = 3;
    }

    private void At_1AM()
    {
        note += Translator.Get("Note.Night2.Part2") + " ";
        Utils.SetCallNote(note);

        AIManager.W_BonnieAI?.Difficulty = 5;
        AIManager.W_ChicaAI?.Difficulty = 3;
        AIManager.W_FoxyAI?.Difficulty = 5;
        AIManager.BalloonBoyAI?.Difficulty = 3;
    }

    private void At_3AM()
    {
        note += Translator.Get("Note.Night2.Part3");
        Utils.SetCallNote(note);

        AIManager.W_FreddyAI?.Difficulty = 10;
        AIManager.W_FoxyAI?.Difficulty = 3;
        AIManager.BalloonBoyAI?.Difficulty = 6;
        AIManager.PuppetAI?.Difficulty = 7;
    }

    private void At_4AM()
    {
        AIManager.W_ChicaAI?.Difficulty = 5;

        AIManager.W_BonnieAI?.TryMoveNextPos();
        AIManager.BalloonBoyAI?.TryMoveNextPos();
    }

    private void At_5AM()
    {
        AIManager.W_FreddyAI?.Difficulty = 6;
        AIManager.W_BonnieAI?.Difficulty = 6;
        AIManager.W_ChicaAI?.Difficulty = 6;
        AIManager.W_FoxyAI?.Difficulty = 5;
        AIManager.BalloonBoyAI?.Difficulty = 8;
    }

    public void OnWin() { }
}