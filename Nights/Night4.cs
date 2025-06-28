using NightmareMode.Helpers;
using NightmareMode.Items.Attributes;
using NightmareMode.Items.Interfaces;
using NightmareMode.Managers;
using UnityEngine;

namespace NightmareMode.Nights;

[RegisterNight]
internal class Night4 : INight
{
    public int Night => 4;

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

    private void At_12AM()
    {
        Utils.SetStartTimeAll(0f);
        AIManager.Toy_BonnieAI?.SetStartTime(30f);
        AIManager.Toy_ChicaAI?.SetStartTime(30f);
        AIManager.MangleAI?.SetStartTime(30f);
        AIManager.W_FreddyAI?.SetStartTime(30f);
        AIManager.W_BonnieAI?.SetStartTime(30f);
        AIManager.W_ChicaAI?.SetStartTime(30f);
        AIManager.W_FoxyAI?.SetStartTime(15f);

        Utils.SetDifficultyAll(0);
        AIManager.Toy_FreddyAI?.SetDifficulty(4);
        AIManager.Toy_BonnieAI?.SetDifficulty(6);
        AIManager.Toy_ChicaAI?.SetDifficulty(6);
        AIManager.MangleAI?.SetDifficulty(10);
        AIManager.BalloonBoyAI?.SetDifficulty(5);
        AIManager.PuppetAI?.SetDifficulty(8);

        AIManager.W_FreddyAI?.SetDifficulty(2);
        AIManager.W_BonnieAI?.SetDifficulty(8);
        AIManager.W_ChicaAI?.SetDifficulty(8);
        AIManager.W_FoxyAI?.SetDifficulty(3);

        NightManager.DelayedNightAction(() =>
        {
            AIManager.W_FreddyAI?.MoveToPos(1);
            AIManager.W_BonnieAI?.MoveToPos(2);
            AIManager.W_ChicaAI?.MoveToPos(4);

            Utils.InvokeRandomAction(
                () => AIManager.W_BonnieAI?.TryMoveNextPos(),
                () => AIManager.W_ChicaAI?.TryMoveNextPos(),
                () =>
                {
                    AIManager.Toy_FreddyAI?.SetDifficulty(3);
                    AIManager.W_ChicaAI?.MoveToPos(2);
                    if (AIManager.BalloonBoyAI != null)
                    {
                        AIManager.BalloonBoyAI.SetStartTime(0);

                        if (!AIManager.BalloonBoyAI.Active)
                        {
                            AudioClip clip = AIManager.BalloonBoyAI.LeaveSounds[UnityEngine.Random.Range(0, AIManager.BalloonBoyAI.LeaveSounds.Length)];
                            AIManager.BalloonBoyAI.LeaveSound.clip = clip;
                            AIManager.BalloonBoyAI.LeaveSound.Play();
                            AIManager.BalloonBoyAI.Timer = UnityEngine.Random.Range(110, 310) / 10f;
                            AIManager.BalloonBoyAI.campos.SetActive(false);
                            AIManager.BalloonBoyAI.CamStatic.wait += 1f;
                            AIManager.BalloonBoyAI.Active = true;
                        }

                        AIManager.BalloonBoyAI.Timer = 0f;
                    }

                    NightManager.DelayedNightAction(() =>
                    {
                        AIManager.W_FoxyAI?.SetActive(true);
                    }, 4f);
                }
            );
        }, 3f);
        NightManager.DelayedNightAction(() =>
        {
            AIManager.MangleAI?.MoveToPos(4);
        }, 15f);
        NightManager.DelayedNightAction(() =>
        {
            AIManager.Toy_BonnieAI?.MoveToPos(3);
            AIManager.Toy_ChicaAI?.MoveToPos(3);
        }, 30f);
    }

    private void At_1AM()
    {
        AIManager.Toy_BonnieAI?.SetDifficulty(8);
        AIManager.Toy_ChicaAI?.SetDifficulty(8);
        AIManager.W_BonnieAI?.SetDifficulty(6);
        AIManager.W_ChicaAI?.SetDifficulty(6);

        AIManager.Toy_FreddyAI?.TryMoveNextPos();

        var toyChicapose = AIManager.Toy_ChicaAI?.GetActivePose();
        if (toyChicapose != null)
        {
            toyChicapose.Timer += UnityEngine.Random.Range(20f, 100f);
        }
    }

    private void At_2AM()
    {
        AIManager.W_FreddyAI?.SetDifficulty(4);
        AIManager.W_BonnieAI?.SetDifficulty(8);
        AIManager.W_ChicaAI?.SetDifficulty(8);
        AIManager.W_FoxyAI?.SetDifficulty(4);

        AIManager.Toy_BonnieAI?.TryMoveNextPos();
        AIManager.W_ChicaAI?.TryMoveNextPos();
    }

    private void At_3AM()
    {
        AIManager.MangleAI?.SetDifficulty(12);
        AIManager.W_FoxyAI?.SetDifficulty(3);
        AIManager.BalloonBoyAI?.SetDifficulty(6);

        AIManager.W_FreddyAI?.TryMoveNextPos();
        AIManager.BalloonBoyAI?.TryMoveNextPos();
    }

    private void At_4AM()
    {
        AIManager.Toy_FreddyAI?.SetDifficulty(7);
        AIManager.BalloonBoyAI?.SetDifficulty(3);
        AIManager.PuppetAI?.SetDifficulty(7);
        AIManager.W_FoxyAI?.SetDifficulty(6);

        Utils.InvokeRandomAction(
            () => AIManager.Toy_BonnieAI?.SetDifficulty(10),
            () => AIManager.Toy_ChicaAI?.SetDifficulty(10)
            );
        Utils.InvokeRandomAction(
            () => AIManager.W_BonnieAI?.SetDifficulty(10),
            () => AIManager.W_ChicaAI?.SetDifficulty(10)
        );

        AIManager.BalloonBoyAI?.TryMoveNextPos();
    }

    private void At_5AM()
    {
        AIManager.Toy_FreddyAI?.SetDifficulty(4);
        AIManager.BalloonBoyAI?.SetDifficulty(0);
        AIManager.PuppetAI?.SetDifficulty(6);
        AIManager.W_FreddyAI?.SetDifficulty(6);
        AIManager.W_FoxyAI?.SetDifficulty(0);

        var manglePose = AIManager.MangleAI?.GetPoseIndex();
        if (manglePose < 5 && manglePose != -1)
        {
            AIManager.MangleAI?.MoveToPos(5);
        }
        AIManager.MangleAI?.TryMoveNextPos();

        NightManager.DelayedNightAction(() =>
        {
            Utils.InvokeRandomAction(
                () => AIManager.Toy_BonnieAI?.TryMoveNextPos(),
                () => AIManager.W_BonnieAI?.TryMoveNextPos()
            );
            Utils.InvokeRandomAction(
                () => AIManager.Toy_ChicaAI?.TryMoveNextPos(),
                () => AIManager.W_ChicaAI?.TryMoveNextPos()
            );
        }, 15f);
    }

    public void OnWin() { }
}
