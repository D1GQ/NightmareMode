using NightmareMode.Helpers;
using NightmareMode.Items.Attributes;
using NightmareMode.Items.Interfaces;
using NightmareMode.Managers;
using NightmareMode.Modules;

namespace NightmareMode.Nights;

[RegisterNight]
internal class Night1 : INight
{
    public int Night => 1;

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

    private void At_12AM()
    {
        note += Translator.Get("Note.Night1.Part1") + " ";
        Utils.SetCallNote(note);
        note += Translator.Get("Note.Night1.Part2") + " ";
        Utils.SetCallNoteDelay(note, 30f);

        Utils.SetStartTimeAll(2.5f);
        AIManager.Toy_FreddyAI?.SetStartTime(30f);
        AIManager.Toy_BonnieAI?.SetStartTime(5f);
        AIManager.Toy_ChicaAI?.SetStartTime(10f);
        AIManager.MangleAI?.SetStartTime(2f);
        AIManager.PuppetAI?.SetStartTime(0f);

        Utils.SetDifficultyAll(0);
        AIManager.Toy_FreddyAI?.SetDifficulty(3);
        AIManager.Toy_BonnieAI?.SetDifficulty(6);
        AIManager.Toy_ChicaAI?.SetDifficulty(4);
        AIManager.MangleAI?.SetDifficulty(10);
        AIManager.PuppetAI?.SetDifficulty(4);

        NightManager.DelayedNightAction(() =>
        {
            AIManager.MangleAI?.MoveToOffice();
        }, 3f);
    }

    private void At_1AM()
    {
        AIManager.Toy_ChicaAI?.SetDifficulty(5);
        AIManager.PuppetAI?.SetDifficulty(5);
    }

    private void At_2AM()
    {
        note += Translator.Get("Note.Night1.Part3") + " ";
        Utils.SetCallNote(note);

        AIManager.MangleAI?.SetDifficulty(5);
        AIManager.Toy_BonnieAI?.SetDifficulty(7);
    }

    private void At_3AM()
    {
        note += Translator.Get("Note.Night1.Part4") + " ";
        Utils.SetCallNote(note);

        AIManager.Toy_BonnieAI?.SetDifficulty(3);
        AIManager.Toy_ChicaAI?.SetDifficulty(8);
        AIManager.Toy_FreddyAI?.SetDifficulty(5);
        AIManager.PuppetAI?.SetDifficulty(6);

        AIManager.Toy_FreddyAI?.TryMoveNextPos();
        AIManager.Toy_ChicaAI?.TryMoveNextPos();
    }

    private void At_4AM()
    {
        note += Translator.Get("Note.Night1.Part5");
        Utils.SetCallNote(note);

        AIManager.MangleAI?.SetDifficulty(12);
        AIManager.Toy_BonnieAI?.SetDifficulty(4);
        AIManager.Toy_ChicaAI?.SetDifficulty(7);
        AIManager.Toy_FreddyAI?.SetDifficulty(3);
    }

    private void At_5AM()
    {
        AIManager.Toy_BonnieAI?.SetDifficulty(6);
        AIManager.Toy_ChicaAI?.SetDifficulty(6);

        AIManager.Toy_FreddyAI?.TryMoveNextPos();
    }

    public void OnWin() { }
}
