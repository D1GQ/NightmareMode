using NightmareMode.Helpers;
using NightmareMode.Items.Attributes;
using NightmareMode.Items.Interfaces;
using NightmareMode.Managers;

namespace NightmareMode.Nights;

[RegisterNight]
internal class CustomNight : INight
{
    public int Night => 7;

    private string note = "";
    public void InitNight()
    {
        note = "";
        foreach (var kvp in CustomNightManager.AILevels)
        {
            note += $"{Utils.GetNickName(kvp.Key)}: {kvp.Value}\n";
        }
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
    }
    public void OnHalfHour(int hour) { }

    private void At_12AM()
    {
        Utils.SetStartTimeAllRandom(0f, 10f);
        AIManager.PuppetAI?.SetStartTime(0f);

        foreach (var kvp in CustomNightManager.AILevels)
        {
            Utils.SetDifficulty(kvp.Key, kvp.Value);
        }
    }

    public void OnWin() { }
}
