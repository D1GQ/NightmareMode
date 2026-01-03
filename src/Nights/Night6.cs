using NightmareMode.Helpers;
using NightmareMode.Items.Interfaces;
using NightmareMode.Managers;

namespace NightmareMode.Nights;

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
    public void At_12AM() { }
    public void At_1AM() { }
    public void At_2AM() { }
    public void At_3AM() { }
    public void At_4AM() { }
    public void At_5AM() { }
    public void OnWin() { }
}
