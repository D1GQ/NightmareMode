using NightmareMode.Attributes;
using NightmareMode.Helpers;
using NightmareMode.Interfaces;
using NightmareMode.Managers;

namespace NightmareMode.Nights;

[RegisterNight]
internal class Night6 : INight
{
    public int Night => 6;

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
            case 3:
                At_3AM();
                break;
            case 5:
                At_5AM();
                break;
        }
    }

    public void OnHalfHour(int hour) { }
    private void At_12AM()
    {
        Utils.SetStartTimeAllRandom(3f, 10f);
        var toyRNG = UnityEngine.Random.Range(5f, 12f);
        AIManager.Toy_BonnieAI?.StartTimer = toyRNG + UnityEngine.Random.Range(-2f, 2f);
        AIManager.Toy_ChicaAI?.StartTimer = toyRNG + UnityEngine.Random.Range(-2f, 2f);
        AIManager.PuppetAI?.StartTimer = 0f;

        Utils.SetDifficultyAll(10);
        AIManager.PuppetAI?.Difficulty = 8;
        AIManager.Toy_FreddyAI?.Difficulty = 15;
        AIManager.MangleAI?.Difficulty = 12;
        AIManager.BalloonBoyAI?.Difficulty = 18;
        AIManager.W_FoxyAI?.Difficulty = 20;
    }
    public void At_3AM()
    {
        AIManager.Toy_BonnieAI?.Difficulty = AIManager.Toy_BonnieAI.Difficulty + UnityEngine.Random.Range(-2, 2);
        AIManager.Toy_ChicaAI?.Difficulty = AIManager.Toy_ChicaAI.Difficulty + UnityEngine.Random.Range(-2, 2);

        AIManager.Toy_FreddyAI?.StartTimer = 18;
        AIManager.MangleAI?.StartTimer = 16;
        AIManager.BalloonBoyAI?.StartTimer = 20;
        AIManager.W_FoxyAI?.StartTimer = 15;
    }
    public void At_5AM()
    {
        Utils.SetDifficultyAll(5);
        AIManager.PuppetAI?.StartTimer = 8;
        AIManager.Toy_FreddyAI?.StartTimer = 15;
        AIManager.MangleAI?.Difficulty = 20;
        AIManager.BalloonBoyAI?.Difficulty = 10;
    }
    public void OnWin() { }
}
