using NightmareMode.Helpers;
using NightmareMode.Items.Attributes;
using NightmareMode.Items.Interfaces;
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
        AIManager.Toy_BonnieAI?.SetStartTime(toyRNG + UnityEngine.Random.Range(-2f, 2f));
        AIManager.Toy_ChicaAI?.SetStartTime(toyRNG + UnityEngine.Random.Range(-2f, 2f));
        AIManager.PuppetAI?.SetStartTime(0f);

        Utils.SetDifficultyAll(10);
        AIManager.Toy_FreddyAI?.SetDifficulty(18);
        AIManager.MangleAI?.SetDifficulty(15);
        AIManager.BalloonBoyAI?.SetDifficulty(18);
        AIManager.W_FoxyAI?.SetDifficulty(20);
    }
    public void At_3AM()
    {
        AIManager.Toy_BonnieAI?.SetDifficulty(AIManager.Toy_BonnieAI.GetDifficulty() + UnityEngine.Random.Range(-2, 2));
        AIManager.Toy_ChicaAI?.SetDifficulty(AIManager.Toy_ChicaAI.GetDifficulty() + UnityEngine.Random.Range(-2, 2));

        AIManager.Toy_FreddyAI?.SetDifficulty(20);
        AIManager.MangleAI?.SetDifficulty(18);
        AIManager.BalloonBoyAI?.SetDifficulty(20);
        AIManager.W_FoxyAI?.SetDifficulty(15);
    }
    public void At_5AM()
    {
        Utils.SetDifficultyAll(5);
        AIManager.PuppetAI?.SetDifficulty(10);
        AIManager.Toy_FreddyAI?.SetDifficulty(20);
        AIManager.MangleAI?.SetDifficulty(20);
        AIManager.BalloonBoyAI?.SetDifficulty(20);
        AIManager.W_FoxyAI?.SetDifficulty(20);
    }
    public void OnWin() { }
}
