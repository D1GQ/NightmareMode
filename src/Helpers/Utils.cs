using NightmareMode.Items.Enums;
using NightmareMode.Managers;
using NightmareMode.Modules;
using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;

namespace NightmareMode.Helpers;

internal static class Utils
{
    internal static Dictionary<string, Sprite> CachedSprites = [];
    private static Dictionary<string, Texture2D> _cachedTextures = new();

    /// <summary>
    /// Loads a sprite from a given path, optionally specifying pixels per unit.
    /// Caches the sprite for future use.
    /// </summary>
    /// <param name="path">The file path of the texture to create the sprite from.</param>
    /// <param name="pixelsPerUnit">The number of pixels per unit for the sprite.</param>
    /// <returns>A Sprite object if successful, or null if it fails to load the sprite.</returns>
    internal static Sprite? LoadSprite(string path, float pixelsPerUnit = 1f)
    {
        try
        {
            if (CachedSprites.TryGetValue(path + pixelsPerUnit, out var sprite))
                return sprite;

            var texture = LoadTextureFromResources(path);
            if (texture == null)
                return null;

            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;

            return CachedSprites[path + pixelsPerUnit] = sprite;
        }
        catch (Exception ex)
        {
            NightmarePlugin.Log.LogError(ex);
            return null;
        }
    }

    /// <summary>
    /// Loads a single sprite from a sprite sheet by its column and row index (0-based).
    /// </summary>
    /// <param name="path">Path to the sprite sheet texture.</param>
    /// <param name="columns">Total columns in the sheet.</param>
    /// <param name="rows">Total rows in the sheet.</param>
    /// <param name="targetSprite">Column index (0-based) of the desired sprite, Row index (0-based) of the desired sprite.</param>
    /// <param name="pixelsPerUnit">Pixels per unit for the sprite.</param>
    /// <param name="padding">Optional padding between sprites in pixels.</param>
    /// <returns>The requested Sprite, or null if out of bounds or failed to load.</returns>
    internal static Sprite? LoadSpriteFromSheet(string path, int columns, int rows, (int col, int row) targetSprite, float pixelsPerUnit = 100f, int padding = 0)
    {
        try
        {
            targetSprite.col--;
            targetSprite.row--;
            string cacheKey = $"{path}_{targetSprite.col}_{targetSprite.row}_{pixelsPerUnit}_{padding}";

            if (CachedSprites.TryGetValue(cacheKey, out var cachedSprite))
                return cachedSprite;

            if (!_cachedTextures.TryGetValue(path, out var texture))
            {
                texture = LoadTextureFromResources(path);
                if (texture == null) return null;
                _cachedTextures[path] = texture;
            }

            if (targetSprite.col >= columns || targetSprite.row >= rows)
            {
                NightmarePlugin.Log.LogWarning($"Sprite index ({targetSprite.col},{targetSprite.row}) is out of bounds!");
                return null;
            }

            int spriteWidth = (texture.width - (padding * (columns - 1))) / columns;
            int spriteHeight = (texture.height - (padding * (rows - 1))) / rows;

            Rect rect = new(
                targetSprite.col * (spriteWidth + padding),
                targetSprite.row * (spriteHeight + padding),
                spriteWidth,
                spriteHeight
            );

            Sprite sprite = Sprite.Create(
                texture,
                rect,
                new Vector2(0.5f, 0.5f),
                pixelsPerUnit
            );
            sprite.hideFlags |= HideFlags.HideAndDontSave;

            CachedSprites[cacheKey] = sprite;
            return sprite;
        }
        catch (Exception ex)
        {
            NightmarePlugin.Log.LogError(ex);
            return null;
        }
    }

    /// <summary>
    /// Loads a texture from embedded resources in the application's assembly.
    /// </summary>
    /// <param name="path">The path to the texture resource.</param>
    /// <returns>A Texture2D object if the texture was loaded successfully, or null if it failed.</returns>
    internal static Texture2D? LoadTextureFromResources(string path)
    {
        try
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            if (stream == null)
                return null;

            var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                if (!texture.LoadImage(ms.ToArray(), false))
                    return null;
            }

            return texture;
        }
        catch (Exception ex)
        {
            NightmarePlugin.Log.LogError(ex);
            return null;
        }
    }

    internal static string HashString(string input)
    {
        using MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    internal static GameObject? FindInactive(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        string[] parts = path.Split('/');
        if (parts.Length == 0)
            return null;

        // Find root (even if inactive)
        Transform? parent = null;
        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name == parts[0])
            {
                parent = root.transform;
                break;
            }
        }
        if (parent == null)
            return null;

        // Traverse path (direct children only)
        for (int i = 1; i < parts.Length; i++)
        {
            parent = parent.Find(parts[i]);
            if (parent == null)
                return null;
        }

        return parent.gameObject;
    }

    internal static void SetCallNote(string note)
    {
        var noteTMP = CatchedSingleton<PauseScript>.Instance?.page2.transform.Find("CallNote")?.GetComponent<TextMeshPro>();
        if (noteTMP != null)
        {
            noteTMP.gameObject.SetActive(true);
            noteTMP.SetText(note);
        }
    }

    internal static void SetCallNoteDelay(string note, float delay)
    {
        CatchedSingleton<PauseScript>.Instance?.StartCoroutine(CoSetCallNoteDelay(note, delay));
    }

    private static IEnumerator CoSetCallNoteDelay(string note, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetCallNote(note);
    }

    internal static void InvokeRandomAction(params Action?[] actions)
    {
        if (actions == null || actions.Length == 0)
            return;

        int randomIndex = UnityEngine.Random.Range(0, actions.Length);
        actions[randomIndex]?.Invoke();
    }

    internal static string GetNickName(AITypes type)
    {
        return type switch
        {
            AITypes.FreddyAI => "Toy Freddy",
            AITypes.BonnieAI => "Toy Bonnie",
            AITypes.ChicaAI => "Toy Chica",
            AITypes.MangleAI => "Mangle",
            AITypes.BBAI => "Balloon Boy",
            AITypes.PuppetAI => "Puppet",
            AITypes.WFreddyAI => "Withered Freddy",
            AITypes.WBonnieAI => "Withered Bonnie",
            AITypes.WChicaAI => "Withered Chica",
            AITypes.FOXYAI => "Withered Foxy",
            _ => "???",
        };
    }

    internal static void SetDifficulty(AITypes type, int diff)
    {
        diff = Mathf.Clamp(diff, 0, 100);
        switch (type)
        {
            case AITypes.FreddyAI:
                AIManager.Toy_FreddyAI?.SetDifficulty(diff);
                break;
            case AITypes.BonnieAI:
                AIManager.Toy_BonnieAI?.SetDifficulty(diff);
                break;
            case AITypes.ChicaAI:
                AIManager.Toy_ChicaAI?.SetDifficulty(diff);
                break;
            case AITypes.MangleAI:
                AIManager.MangleAI?.SetDifficulty(diff);
                break;
            case AITypes.BBAI:
                AIManager.BalloonBoyAI?.SetDifficulty(diff);
                break;
            case AITypes.PuppetAI:
                AIManager.PuppetAI?.SetDifficulty(diff);
                break;
            case AITypes.WFreddyAI:
                AIManager.W_FreddyAI?.SetDifficulty(diff);
                break;
            case AITypes.WBonnieAI:
                AIManager.W_BonnieAI?.SetDifficulty(diff);
                break;
            case AITypes.WChicaAI:
                AIManager.W_ChicaAI?.SetDifficulty(diff);
                break;
            case AITypes.FOXYAI:
                AIManager.W_FoxyAI?.SetDifficulty(diff);
                break;
        }
    }
    internal static void SetDifficulty(this AnimatronicAIScript AI, int diff)
    {
        diff = Mathf.Clamp(diff, 0, 100);
        AI.enabled = diff > 0;
        AI.Difficulty = diff;

        var positionScript = AI.FirstPosition;
        while (positionScript != null)
        {
            var oldPositionScript = positionScript;
            oldPositionScript.Dif = diff;
            positionScript = oldPositionScript.NextPos;
        }
    }
    internal static void SetDifficulty(this ToyFreddyBrain AI, int diff)
    {
        diff = Mathf.Clamp(diff, 0, 100);
        AI.enabled = diff > 0;
        AI.Difficulty = diff;
    }
    internal static void SetDifficulty(this BBAIScript AI, int diff)
    {
        diff = Mathf.Clamp(diff, 0, 100);
        AI.enabled = diff > 0;
        AI.Difficulty = diff;
    }
    internal static void SetDifficulty(this PuppetScript AI, int diff)
    {
        diff = Mathf.Clamp(diff, 0, 100);
        AI.enabled = diff > 0;
        AI.dif = diff;
    }
    internal static void SetDifficulty(this FoxyBrainScript AI, int diff)
    {
        diff = Mathf.Clamp(diff, 0, 100);
        if (diff <= 0)
        {
            AI.SetActive(false);
        }
        AI.enabled = diff > 0;
        AI.Difficulty = diff;
    }
    internal static void SetDifficultyAll(int diff)
    {
        AIManager.Toy_FreddyAI?.SetDifficulty(diff);
        AIManager.Toy_BonnieAI?.SetDifficulty(diff);
        AIManager.Toy_ChicaAI?.SetDifficulty(diff);
        AIManager.MangleAI?.SetDifficulty(diff);
        AIManager.BalloonBoyAI?.SetDifficulty(diff);
        AIManager.PuppetAI?.SetDifficulty(diff);
        AIManager.W_FreddyAI?.SetDifficulty(diff);
        AIManager.W_BonnieAI?.SetDifficulty(diff);
        AIManager.W_ChicaAI?.SetDifficulty(diff);
        AIManager.W_FoxyAI?.SetDifficulty(diff);
    }
    internal static void SetDifficultyAllRandom(int min, int max)
    {
        AIManager.Toy_FreddyAI?.SetDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.Toy_BonnieAI?.SetDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.Toy_ChicaAI?.SetDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.MangleAI?.SetDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.BalloonBoyAI?.SetDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.PuppetAI?.SetDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.W_FreddyAI?.SetDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.W_BonnieAI?.SetDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.W_ChicaAI?.SetDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.W_FoxyAI?.SetDifficulty(UnityEngine.Random.Range(min, max));
    }

    internal static void AddDifficulty(this AnimatronicAIScript AI, int diff) => AI.SetDifficulty(AI.GetDifficulty() + diff);
    internal static void AddDifficulty(this ToyFreddyBrain AI, int diff) => AI.SetDifficulty(AI.GetDifficulty() + diff);
    internal static void AddDifficulty(this BBAIScript AI, int diff) => AI.SetDifficulty(AI.GetDifficulty() + diff);
    internal static void AddDifficulty(this PuppetScript AI, int diff) => AI.SetDifficulty(AI.GetDifficulty() + diff);
    internal static void AddDifficulty(this FoxyBrainScript AI, int diff) => AI.SetDifficulty(AI.GetDifficulty() + diff);
    internal static void AddDifficultyAll(int diff)
    {
        AIManager.Toy_FreddyAI?.AddDifficulty(diff);
        AIManager.Toy_BonnieAI?.AddDifficulty(diff);
        AIManager.Toy_ChicaAI?.AddDifficulty(diff);
        AIManager.MangleAI?.AddDifficulty(diff);
        AIManager.BalloonBoyAI?.AddDifficulty(diff);
        AIManager.PuppetAI?.AddDifficulty(diff);
        AIManager.W_FreddyAI?.AddDifficulty(diff);
        AIManager.W_BonnieAI?.AddDifficulty(diff);
        AIManager.W_ChicaAI?.AddDifficulty(diff);
        AIManager.W_FoxyAI?.AddDifficulty(diff);
    }
    internal static void AddDifficultyAllRandom(int min, int max)
    {
        AIManager.Toy_FreddyAI?.AddDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.Toy_BonnieAI?.AddDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.Toy_ChicaAI?.AddDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.MangleAI?.AddDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.BalloonBoyAI?.AddDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.PuppetAI?.AddDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.W_FreddyAI?.AddDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.W_BonnieAI?.AddDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.W_ChicaAI?.AddDifficulty(UnityEngine.Random.Range(min, max));
        AIManager.W_FoxyAI?.AddDifficulty(UnityEngine.Random.Range(min, max));
    }

    internal static int GetDifficulty(AITypes type)
    {
        return type switch
        {
            AITypes.FreddyAI => AIManager.Toy_FreddyAI?.GetDifficulty() ?? -1,
            AITypes.BonnieAI => AIManager.Toy_BonnieAI?.GetDifficulty() ?? -1,
            AITypes.ChicaAI => AIManager.Toy_ChicaAI?.GetDifficulty() ?? -1,
            AITypes.MangleAI => AIManager.MangleAI?.GetDifficulty() ?? -1,
            AITypes.BBAI => AIManager.BalloonBoyAI?.GetDifficulty() ?? -1,
            AITypes.PuppetAI => AIManager.PuppetAI?.GetDifficulty() ?? -1,
            AITypes.WFreddyAI => AIManager.W_FreddyAI?.GetDifficulty() ?? -1,
            AITypes.WBonnieAI => AIManager.W_BonnieAI?.GetDifficulty() ?? -1,
            AITypes.WChicaAI => AIManager.W_ChicaAI?.GetDifficulty() ?? -1,
            AITypes.FOXYAI => AIManager.W_FoxyAI?.GetDifficulty() ?? -1,
            _ => -1,
        };
    }
    internal static int GetDifficulty(this AnimatronicAIScript AI) => (int)AI.Difficulty;
    internal static int GetDifficulty(this ToyFreddyBrain AI) => (int)AI.Difficulty;
    internal static int GetDifficulty(this BBAIScript AI) => (int)AI.Difficulty;
    internal static int GetDifficulty(this PuppetScript AI) => (int)AI.dif;
    internal static int GetDifficulty(this FoxyBrainScript AI) => (int)AI.Difficulty;

    internal static void SetStartTime(AITypes type, float time)
    {
        switch (type)
        {
            case AITypes.FreddyAI:
                AIManager.Toy_FreddyAI?.SetStartTime(time);
                break;
            case AITypes.BonnieAI:
                AIManager.Toy_BonnieAI?.SetStartTime(time);
                break;
            case AITypes.ChicaAI:
                AIManager.Toy_ChicaAI?.SetStartTime(time);
                break;
            case AITypes.MangleAI:
                AIManager.MangleAI?.SetStartTime(time);
                break;
            case AITypes.BBAI:
                AIManager.BalloonBoyAI?.SetStartTime(time);
                break;
            case AITypes.PuppetAI:
                AIManager.PuppetAI?.SetStartTime(time);
                break;
            case AITypes.WFreddyAI:
                AIManager.W_FreddyAI?.SetStartTime(time);
                break;
            case AITypes.WBonnieAI:
                AIManager.W_BonnieAI?.SetStartTime(time);
                break;
            case AITypes.WChicaAI:
                AIManager.W_ChicaAI?.SetStartTime(time);
                break;
            case AITypes.FOXYAI:
                AIManager.W_FoxyAI?.SetStartTime(time);
                break;
        }
    }
    internal static void SetStartTime(this AnimatronicAIScript AI, float time)
    {
        AI.StartTimer = time;
    }
    internal static void SetStartTime(this ToyFreddyBrain AI, float time)
    {
        AI.StartTimer = time;
    }
    internal static void SetStartTime(this BBAIScript AI, float time)
    {
        AI.StartTimer = time;
    }
    internal static void SetStartTime(this PuppetScript AI, float time)
    {
        AI.start = time;
    }
    internal static void SetStartTime(this FoxyBrainScript AI, float time)
    {
        AI.StartTimer = time;
    }

    internal static void SetStartTimeAll(float time)
    {
        AIManager.Toy_FreddyAI?.SetStartTime(time);
        AIManager.Toy_BonnieAI?.SetStartTime(time);
        AIManager.Toy_ChicaAI?.SetStartTime(time);
        AIManager.MangleAI?.SetStartTime(time);
        AIManager.BalloonBoyAI?.SetStartTime(time);
        AIManager.PuppetAI?.SetStartTime(time);
        AIManager.W_FreddyAI?.SetStartTime(time);
        AIManager.W_BonnieAI?.SetStartTime(time);
        AIManager.W_ChicaAI?.SetStartTime(time);
        AIManager.W_FoxyAI?.SetStartTime(time);
    }

    internal static void SetStartTimeSelected(float time, params AITypes[] AIs)
    {
        var AIsHashSet = AIs.ToHashSet();
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.Toy_FreddyAI?.SetStartTime(time);
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.Toy_BonnieAI?.SetStartTime(time);
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.Toy_ChicaAI?.SetStartTime(time);
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.MangleAI?.SetStartTime(time);
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.BalloonBoyAI?.SetStartTime(time);
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.PuppetAI?.SetStartTime(time);
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.W_FreddyAI?.SetStartTime(time);
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.W_BonnieAI?.SetStartTime(time);
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.W_ChicaAI?.SetStartTime(time);
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.W_FoxyAI?.SetStartTime(time);
    }

    internal static void SetStartTimeAllRandom(float min, float max)
    {
        AIManager.Toy_FreddyAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        AIManager.Toy_BonnieAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        AIManager.Toy_ChicaAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        AIManager.MangleAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        AIManager.BalloonBoyAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        AIManager.PuppetAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        AIManager.W_FreddyAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        AIManager.W_BonnieAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        AIManager.W_ChicaAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        AIManager.W_FoxyAI?.SetStartTime(UnityEngine.Random.Range(min, max));
    }

    internal static void SetStartTimeSelectedRandom(float min, float max, params AITypes[] AIs)
    {
        var AIsHashSet = AIs.ToHashSet();
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.Toy_FreddyAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.Toy_BonnieAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.Toy_ChicaAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.MangleAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.BalloonBoyAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.PuppetAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.W_FreddyAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.W_BonnieAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.W_ChicaAI?.SetStartTime(UnityEngine.Random.Range(min, max));
        if (AIsHashSet.Contains(AITypes.FreddyAI))
            AIManager.W_FoxyAI?.SetStartTime(UnityEngine.Random.Range(min, max));
    }

    internal static void MoveToPos(this AnimatronicAIScript AI, int index) => AI.MoveToPos(AI.GetPose(index));

    internal static void MoveToPos(this AnimatronicAIScript AI, AnimatronicPositionScript? positionScript)
    {
        var pos = AI.GetActivePose();
        if (pos != null)
        {
            pos.Static.wait += 1f;
            pos.Model.SetActive(false);
            pos.ACTIVE = false;
        }
        else
        {
            if (!AI.OffStage)
            {
                if (!AI.Withered)
                {
                    AI.Stage.Lower();
                }

                if (AI.Withered || AI.Stage.down)
                {
                    AI.StageModel.SetActive(false);
                    AI.StageStatic.wait += 1f;
                    AI.OffStage = true;
                    AI.StartTimer = 0f;
                }
            }
        }

        positionScript?.Activate();
    }

    internal static bool IsInOffice(this AnimatronicAIScript AI) => AI.GetPoses().Last()?.OfficePosition?.activeInHierarchy == true;
    internal static void MoveToOffice(this AnimatronicAIScript AI)
    {
        AI.MoveToPos(null);
        AI.GetPoses().Last()?.OfficePosition?.SetActive(true);
    }

    internal static void TryMoveNextPos(this AnimatronicAIScript AI, bool force = false)
    {
        var pos = AI.GetActivePose();
        if (pos != null)
        {
            if (!force)
            {
                pos.Timer = 0;
            }
            else
            {
                pos.NextPos?.Activate();
                pos.Static.wait += 1f;
                pos.Model.SetActive(false);
                pos.ACTIVE = false;
            }
        }
        else
        {
            AI.SetStartTime(0f);
        }
    }

    internal static void TryMoveNextPos(this ToyFreddyBrain AI, bool force = false)
    {
        static void TryShutDownBreaker(ToyFreddyPositionScript breaker)
        {
            if (breaker.Arrived)
            {
                breaker.OutageTimer = 0f;
            }
        }

        if (!AI.Active)
        {
            AI.StartTimer = 0f;
        }
        else if (AI.Moving)
        {
            AI.MoveTimer = 0f;
        }
        else
        {
            if (!force) return;

            TryShutDownBreaker(AI.choice1);
            TryShutDownBreaker(AI.choice2);
            TryShutDownBreaker(AI.choice3);
            TryShutDownBreaker(AI.choice4);
        }
    }

    internal static void TryMoveNextPos(this BBAIScript AI, bool force = false)
    {
        if (!AI.Active)
        {
            AI.StartTimer = 0f;
        }
        else if (!AI.BB1.Active && !!AI.BB2.Active)
        {
            AI.Timer = 0f;
        }
        else
        {
            if (!force) return;
            if (AI.BB1.Active)
            {
                AI.BB1.prog = 0f;
                AI.BB1.timer = 0f;
            }
            else if (AI.BB2.Active)
            {
                AI.BB2.prog = 0f;
                AI.BB2.timer = 0f;
            }
        }
    }

    internal static float GetPoseTimer(this AnimatronicPositionScript animatronicPosition) => animatronicPosition.Timer;
    internal static void SetPoseTimer(this AnimatronicPositionScript animatronicPosition, float time) => animatronicPosition.Timer = time;
    internal static void AddPoseTimer(this AnimatronicPositionScript animatronicPosition, float time) => animatronicPosition.Timer += time;

    internal static AnimatronicPositionScript? GetActivePose(this AnimatronicAIScript AI)
    {
        var pos = AI.FirstPosition;
        while (pos != null)
        {
            if (pos.Model.activeInHierarchy)
            {
                return pos;
            }

            pos = pos.NextPos;
        }
        return null;
    }

    internal static int GetPoseIndex(this AnimatronicAIScript AI) => AI.GetPoseIndex(AI.GetActivePose());

    internal static int GetPoseIndex(this AnimatronicAIScript AI, AnimatronicPositionScript? positionScript)
    {
        if (positionScript == null) return -1;
        var pos = AI.FirstPosition;
        int index = 0;
        while (pos != null)
        {
            if (pos == positionScript)
            {
                return index;
            }

            index++;
            pos = pos.NextPos;
        }
        return -1;
    }

    internal static AnimatronicPositionScript[] GetPoses(this AnimatronicAIScript AI)
    {
        List<AnimatronicPositionScript> poses = [];
        var pos = AI.FirstPosition;
        while (pos != null)
        {
            poses.Add(pos);
            pos = pos.NextPos;
        }
        return [.. poses];
    }

    internal static AnimatronicPositionScript? GetPose(this AnimatronicAIScript AI, int index)
    {
        if (index < 0 || AI.FirstPosition == null)
            return null;

        var pos = AI.FirstPosition;
        for (int i = 0; i < index; i++)
        {
            pos = pos.NextPos;
            if (pos == null)
                return null;
        }
        return pos;
    }

    internal static int GetBreakerIndex(this ToyFreddyBrain AI)
    {
        if (AI.choice1.Arrived)
        {
            return 1;
        }
        else if (AI.choice2.Arrived)
        {
            return 2;
        }
        else if (AI.choice3.Arrived)
        {
            return 3;
        }
        else if (AI.choice4.Arrived)
        {
            return 4;
        }
        return 0;
    }

    internal static void SetActive(this FoxyBrainScript AI, bool active)
    {
        if (AI.Active == active) return;

        if (!AI.Active && active)
        {
            AI.StartTimer = 0f;
        }
        else if (AI.Active && !active)
        {
            AI.StartTimer = UnityEngine.Random.Range(610, 810) / 10f - AI.Difficulty * 2f;
        }

        AI.Foxy.SetBool("Active", active);
        AI.Active = active;
    }
}
