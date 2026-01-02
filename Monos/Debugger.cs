using NightmareMode.Helpers;
using NightmareMode.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NightmareMode.Monos;

internal class Debugger : MonoSingleton<Debugger>
{
    private bool _showNightInfo;
    internal bool _godMode;

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "title")
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U))
            {
                NightUI.UnloadAllNights();
            }
        }

        if (!NightmarePlugin.isDebug) return;

        if (SceneManager.GetActiveScene().name == "Nights")
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _showNightInfo = !_showNightInfo;
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                _godMode = !_godMode;
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                var clockScript = CatchedSingleton<ClockScript>.Instance;
                if (clockScript != null)
                {
                    clockScript.timer = ((60f * ((int)clockScript.Hours + 1)) - -0.1f);
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                var clockScript = CatchedSingleton<ClockScript>.Instance;
                if (clockScript != null && NightManager.Current != null)
                {
                    clockScript.timer = (60f * NightManager.Current.Hours) - 0.1f;
                }
            }
        }
        else
        {
            _showNightInfo = false;
            _godMode = false;
        }
    }

    private void OnGUI()
    {
        if (!NightmarePlugin.isDebug) return;

        string textInfo = "";

        if (SceneManager.GetActiveScene().name == "Nights")
        {
            if (_showNightInfo)
            {
                textInfo += $"Night: {BrainScript.night}\n";

                float timer = CatchedSingleton<ClockScript>.Instance?.timer ?? 0f;
                int hours = Mathf.FloorToInt(timer / 60f) % 12;
                int minutes = Mathf.FloorToInt(timer % 60f);
                if (hours == 0) hours = 12;
                textInfo += $"Time: {hours:00}:{minutes:00}\n";

                textInfo += "--------------------------\n";

                if (AIManager.Toy_FreddyAI?.Difficulty > 0)
                {
                    var TFindex = AIManager.Toy_FreddyAI?.GetBreakerIndex();
                    string TFpose = TFindex == 0 ? AIManager.Toy_FreddyAI?.Active == false ?
                        $"Pos=Stage({(int)AIManager.Toy_FreddyAI.StartTimer})"
                        : $"Waiting={(int)(AIManager.Toy_FreddyAI?.MoveTimer ?? 0)}"
                        : $"Pos=Breaker({TFindex})";
                    textInfo += $"ToyFreddy: AI={AIManager.Toy_FreddyAI?.Difficulty}, {TFpose}\n";
                }

                if (AIManager.Toy_BonnieAI?.Difficulty > 0)
                {
                    textInfo += $"ToyBonnie: AI={AIManager.Toy_BonnieAI?.Difficulty}, Pos={AIManager.Toy_BonnieAI?.GetPoseIndex()}\n";
                }

                if (AIManager.Toy_ChicaAI?.Difficulty > 0)
                {
                    textInfo += $"ToyChica: AI={AIManager.Toy_ChicaAI?.Difficulty}, Pos={AIManager.Toy_ChicaAI?.GetPoseIndex()}\n";
                }

                if (AIManager.MangleAI?.Difficulty > 0)
                {
                    string Mpose = AIManager.MangleAI?.GetPoses().Last().OfficePosition.activeInHierarchy == false ? AIManager.MangleAI?.GetPoseIndex().ToString() ?? "-1" : "Hall";
                    textInfo += $"Mangle: AI={AIManager.MangleAI?.Difficulty}, Pos={Mpose}\n";
                }

                if (AIManager.BalloonBoyAI?.Difficulty > 0)
                {
                    string ventInfo = $"Waiting={(int)AIManager.BalloonBoyAI.Timer}";
                    BBVentScript? BBVent = AIManager.BalloonBoyAI.BB1.Active ? AIManager.BalloonBoyAI.BB1 : null;
                    BBVent ??= AIManager.BalloonBoyAI.BB2.Active ? AIManager.BalloonBoyAI.BB2 : null;
                    if (BBVent != null)
                    {
                        ventInfo = $"Vent={(int)BBVent.timer}, Prog={BBVent.prog}";
                    }

                    textInfo += $"BB: AI={AIManager.BalloonBoyAI?.Difficulty}, {ventInfo}\n";
                }

                if (AIManager.PuppetAI?.dif > 0 && AIManager.PuppetAI.start <= 0f)
                {
                    textInfo += $"Puppet: AI={AIManager.PuppetAI?.dif ?? 0}, Time={(int)(AIManager.PuppetAI?.timer ?? 0f)}\n";
                }

                if (AIManager.W_FreddyAI?.Difficulty > 0)
                {
                    textInfo += $"WFreddy: AI={AIManager.W_FreddyAI?.Difficulty}, Pos={AIManager.W_FreddyAI?.GetPoseIndex()}\n";
                }

                if (AIManager.W_BonnieAI?.Difficulty > 0)
                {
                    textInfo += $"WBonnie: AI={AIManager.W_BonnieAI?.Difficulty}, Pos={AIManager.W_BonnieAI?.GetPoseIndex()}\n";
                }

                if (AIManager.W_ChicaAI?.Difficulty > 0)
                {
                    textInfo += $"WChica: AI={AIManager.W_ChicaAI?.Difficulty}, Pos={AIManager.W_ChicaAI?.GetPoseIndex()}\n";
                }

                if (AIManager.W_FoxyAI?.Difficulty > 0)
                {
                    textInfo += $"WFoxy: AI={AIManager.W_FoxyAI?.Difficulty}, Hall={AIManager.W_FoxyAI?.Active}\n";
                }

                textInfo += "--------------------------\n";

                bool cheatsOn = _godMode;
                if (cheatsOn)
                {
                    textInfo += "Cheats:\n";
                }
                if (_godMode)
                {
                    textInfo += "(God Mode)\n";
                }
            }
        }

        GUIStyle style = new(GUI.skin.label)
        {
            fontSize = 24,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperLeft,
            wordWrap = false,
            normal = new GUIStyleState { textColor = Color.white }
        };

        GUIStyle shadowStyle = new GUIStyle(style)
        {
            normal = new GUIStyleState { textColor = Color.black }
        };

        float width = 400f;
        float height = style.CalcHeight(new GUIContent(textInfo), width);

        float x = 10f;
        float y = 50f;

        Rect rect = new(x, y, width, height);

        GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), textInfo, shadowStyle);
        GUI.Label(rect, textInfo, style);
    }
}
