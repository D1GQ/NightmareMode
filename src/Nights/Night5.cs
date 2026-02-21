using NightmareMode.Attributes;
using NightmareMode.Helpers;
using NightmareMode.Interfaces;
using NightmareMode.Managers;

namespace NightmareMode.Nights;

[RegisterNight]
internal class Night5 : INight
{
    public int Night => 5;

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
        }

        if (hour >= 6 && hour < 12) return;

        NightManager.PowerSurge();
    }

    public void OnHalfHour(int hour)
    {
        if (hour >= 6 && hour < 12) return;

        NightManager.PowerSurgeOut();
    }

    private void At_12AM()
    {
        Utils.SetStartTimeAllRandom(3f, 10f);
        var toyRNG = UnityEngine.Random.Range(3f, 10f);
        AIManager.Toy_BonnieAI?.StartTimer = toyRNG + UnityEngine.Random.Range(-1.5f, 1.5f);
        AIManager.Toy_ChicaAI?.StartTimer = toyRNG + UnityEngine.Random.Range(-1.5f, 1.5f);
        AIManager.PuppetAI?.StartTimer = 0f;

        Utils.SetDifficultyAll(15);
        AIManager.PuppetAI?.Difficulty = 8;
        AIManager.W_FoxyAI?.Difficulty = 17;
    }

    public void OnWin() { }
}
