using NightmareMode.Attributes;
using NightmareMode.Helpers;
using NightmareMode.Interfaces;
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
        AIManager.Toy_BonnieAI?.StartTimer = 30f;
        AIManager.Toy_ChicaAI?.StartTimer = 30f;
        AIManager.MangleAI?.StartTimer = 30f;
        AIManager.W_FreddyAI?.StartTimer = 30f;
        AIManager.W_BonnieAI?.StartTimer = 30f;
        AIManager.W_ChicaAI?.StartTimer = 30f;
        AIManager.W_FoxyAI?.StartTimer = 15f;

        Utils.SetDifficultyAll(0);
        AIManager.Toy_FreddyAI?.Difficulty = 4;
        AIManager.Toy_BonnieAI?.Difficulty = 6;
        AIManager.Toy_ChicaAI?.Difficulty = 6;
        AIManager.MangleAI?.Difficulty = 10;
        AIManager.BalloonBoyAI?.Difficulty = 5;
        AIManager.PuppetAI?.Difficulty = 8;

        AIManager.W_FreddyAI?.Difficulty = 2;
        AIManager.W_BonnieAI?.Difficulty = 8;
        AIManager.W_ChicaAI?.Difficulty = 8;
        AIManager.W_FoxyAI?.Difficulty = 3;

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
                    AIManager.Toy_FreddyAI?.Difficulty = 3;
                    AIManager.W_ChicaAI?.MoveToPos(2);
                    if (AIManager.BalloonBoyAI != null)
                    {
                        AIManager.BalloonBoyAI.StartTimer = 0f;

                        if (!AIManager.BalloonBoyAI.Active)
                        {
                            AudioClip clip = AIManager.BalloonBoyAI.AI.LeaveSounds[UnityEngine.Random.Range(0, AIManager.BalloonBoyAI.AI.LeaveSounds.Length)];
                            AIManager.BalloonBoyAI.AI.LeaveSound.clip = clip;
                            AIManager.BalloonBoyAI.AI.LeaveSound.Play();
                            AIManager.BalloonBoyAI.AI.Timer = UnityEngine.Random.Range(110, 310) / 10f;
                            AIManager.BalloonBoyAI.AI.campos.SetActive(false);
                            AIManager.BalloonBoyAI.AI.CamStatic.wait += 1f;
                            AIManager.BalloonBoyAI.AI.Active = true;
                        }

                        AIManager.BalloonBoyAI.StartTimer = 0f;
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
        AIManager.Toy_BonnieAI?.Difficulty = 8;
        AIManager.Toy_ChicaAI?.Difficulty = 8;
        AIManager.W_BonnieAI?.Difficulty = 6;
        AIManager.W_ChicaAI?.Difficulty = 6;

        AIManager.Toy_FreddyAI?.TryMoveNextPos();

        var toyChicapose = AIManager.Toy_ChicaAI?.GetActivePose();
        if (toyChicapose != null)
        {
            toyChicapose.Timer += UnityEngine.Random.Range(20f, 100f);
        }
    }

    private void At_2AM()
    {
        AIManager.W_FreddyAI?.Difficulty = 4;
        AIManager.W_BonnieAI?.Difficulty = 8;
        AIManager.W_ChicaAI?.Difficulty = 8;
        AIManager.W_FoxyAI?.Difficulty = 4;

        AIManager.Toy_BonnieAI?.TryMoveNextPos();
        AIManager.W_ChicaAI?.TryMoveNextPos();
    }

    private void At_3AM()
    {
        AIManager.MangleAI?.Difficulty = 12;
        AIManager.W_FoxyAI?.Difficulty = 3;
        AIManager.BalloonBoyAI?.Difficulty = 6;

        AIManager.W_FreddyAI?.TryMoveNextPos();
        AIManager.BalloonBoyAI?.TryMoveNextPos();
    }

    private void At_4AM()
    {
        AIManager.Toy_FreddyAI?.Difficulty = 7;
        AIManager.BalloonBoyAI?.Difficulty = 3;
        AIManager.PuppetAI?.Difficulty = 7;
        AIManager.W_FoxyAI?.Difficulty = 6;

        Utils.InvokeRandomAction(
            () => AIManager.Toy_BonnieAI?.Difficulty = 10,
            () => AIManager.Toy_ChicaAI?.Difficulty = 10
            );
        Utils.InvokeRandomAction(
            () => AIManager.W_BonnieAI?.Difficulty = 10,
            () => AIManager.W_ChicaAI?.Difficulty = 10
        );

        AIManager.BalloonBoyAI?.TryMoveNextPos();
    }

    private void At_5AM()
    {
        AIManager.Toy_FreddyAI?.Difficulty = 4;
        AIManager.BalloonBoyAI?.Difficulty = 0;
        AIManager.PuppetAI?.Difficulty = 6;
        AIManager.W_FreddyAI?.Difficulty = 6;
        AIManager.W_FoxyAI?.Difficulty = 0;

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
