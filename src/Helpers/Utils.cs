using NightmareMode.Enums;
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

    private static GameObject? _cameras;
    private static GameObject Cameras
    {
        get
        {
            if (_cameras == null)
            {
                _cameras = GameObject.Find("Alive/GAMPLAYCOMPONENTS/Cameras");
            }

            return _cameras;
        }
    }

    internal static bool InCameras()
    {
        var cameras = Cameras;
        return cameras != null && cameras.activeInHierarchy;
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
            AITypes.FreddyAI => Translator.Get("Character.ToyFreddy"),
            AITypes.BonnieAI => Translator.Get("Character.ToyBonnie"),
            AITypes.ChicaAI => Translator.Get("Character.ToyChica"),
            AITypes.MangleAI => Translator.Get("Character.Mangle"),
            AITypes.BBAI => Translator.Get("Character.BalloonBoy"),
            AITypes.PuppetAI => Translator.Get("Character.Puppet"),
            AITypes.WFreddyAI => Translator.Get("Character.Freddy"),
            AITypes.WBonnieAI => Translator.Get("Character.Bonnie"),
            AITypes.WChicaAI => Translator.Get("Character.Chica"),
            AITypes.FOXYAI => Translator.Get("Character.Foxy"),
            _ => "???",
        };
    }

    internal static void SetDifficulty(AITypes type, int diff)
    {
        diff = Mathf.Clamp(diff, 0, 100);
        switch (type)
        {
            case AITypes.FreddyAI:
                AIManager.Toy_FreddyAI?.Difficulty = diff;
                break;
            case AITypes.BonnieAI:
                AIManager.Toy_BonnieAI?.Difficulty = diff;
                break;
            case AITypes.ChicaAI:
                AIManager.Toy_ChicaAI?.Difficulty = diff;
                break;
            case AITypes.MangleAI:
                AIManager.MangleAI?.Difficulty = diff;
                break;
            case AITypes.BBAI:
                AIManager.BalloonBoyAI?.Difficulty = diff;
                break;
            case AITypes.PuppetAI:
                AIManager.PuppetAI?.Difficulty = diff;
                break;
            case AITypes.WFreddyAI:
                AIManager.W_FreddyAI?.Difficulty = diff;
                break;
            case AITypes.WBonnieAI:
                AIManager.W_BonnieAI?.Difficulty = diff;
                break;
            case AITypes.WChicaAI:
                AIManager.W_ChicaAI?.Difficulty = diff;
                break;
            case AITypes.FOXYAI:
                AIManager.W_FoxyAI?.Difficulty = diff;
                break;
        }
    }

    internal static void SetDifficultyAll(int diff)
    {
        AIManager.Toy_FreddyAI?.Difficulty = diff;
        AIManager.Toy_BonnieAI?.Difficulty = diff;
        AIManager.Toy_ChicaAI?.Difficulty = diff;
        AIManager.MangleAI?.Difficulty = diff;
        AIManager.BalloonBoyAI?.Difficulty = diff;
        AIManager.PuppetAI?.Difficulty = diff;
        AIManager.W_FreddyAI?.Difficulty = diff;
        AIManager.W_BonnieAI?.Difficulty = diff;
        AIManager.W_ChicaAI?.Difficulty = diff;
        AIManager.W_FoxyAI?.Difficulty = diff;
    }

    internal static void SetDifficultyAllRandom(int min, int max)
    {
        AIManager.Toy_FreddyAI?.Difficulty = UnityEngine.Random.Range(min, max);
        AIManager.Toy_BonnieAI?.Difficulty = UnityEngine.Random.Range(min, max);
        AIManager.Toy_ChicaAI?.Difficulty = UnityEngine.Random.Range(min, max);
        AIManager.MangleAI?.Difficulty = UnityEngine.Random.Range(min, max);
        AIManager.BalloonBoyAI?.Difficulty = UnityEngine.Random.Range(min, max);
        AIManager.PuppetAI?.Difficulty = UnityEngine.Random.Range(min, max);
        AIManager.W_FreddyAI?.Difficulty = UnityEngine.Random.Range(min, max);
        AIManager.W_BonnieAI?.Difficulty = UnityEngine.Random.Range(min, max);
        AIManager.W_ChicaAI?.Difficulty = UnityEngine.Random.Range(min, max);
        AIManager.W_FoxyAI?.Difficulty = UnityEngine.Random.Range(min, max);
    }

    internal static void AddDifficultyAll(int diff)
    {
        AIManager.Toy_FreddyAI?.Difficulty += diff;
        AIManager.Toy_BonnieAI?.Difficulty += diff;
        AIManager.Toy_ChicaAI?.Difficulty += diff;
        AIManager.MangleAI?.Difficulty += diff;
        AIManager.BalloonBoyAI?.Difficulty += diff;
        AIManager.PuppetAI?.Difficulty += diff;
        AIManager.W_FreddyAI?.Difficulty += diff;
        AIManager.W_BonnieAI?.Difficulty += diff;
        AIManager.W_ChicaAI?.Difficulty += diff;
        AIManager.W_FoxyAI?.Difficulty += diff;
    }
    internal static void AddDifficultyAllRandom(int min, int max)
    {
        AIManager.Toy_FreddyAI?.Difficulty += UnityEngine.Random.Range(min, max);
        AIManager.Toy_BonnieAI?.Difficulty += UnityEngine.Random.Range(min, max);
        AIManager.Toy_ChicaAI?.Difficulty += UnityEngine.Random.Range(min, max);
        AIManager.MangleAI?.Difficulty += UnityEngine.Random.Range(min, max);
        AIManager.BalloonBoyAI?.Difficulty += UnityEngine.Random.Range(min, max);
        AIManager.PuppetAI?.Difficulty += UnityEngine.Random.Range(min, max);
        AIManager.W_FreddyAI?.Difficulty += UnityEngine.Random.Range(min, max);
        AIManager.W_BonnieAI?.Difficulty += UnityEngine.Random.Range(min, max);
        AIManager.W_ChicaAI?.Difficulty += UnityEngine.Random.Range(min, max);
        AIManager.W_FoxyAI?.Difficulty += UnityEngine.Random.Range(min, max);
    }

    internal static void SetStartTimeAll(float time)
    {
        AIManager.Toy_FreddyAI?.StartTimer = time;
        AIManager.Toy_BonnieAI?.StartTimer = time;
        AIManager.Toy_ChicaAI?.StartTimer = time;
        AIManager.MangleAI?.StartTimer = time;
        AIManager.BalloonBoyAI?.StartTimer = time;
        AIManager.PuppetAI?.StartTimer = time;
        AIManager.W_FreddyAI?.StartTimer = time;
        AIManager.W_BonnieAI?.StartTimer = time;
        AIManager.W_ChicaAI?.StartTimer = time;
        AIManager.W_FoxyAI?.StartTimer = time;
    }

    internal static void SetStartTimeAllRandom(float min, float max)
    {
        AIManager.Toy_FreddyAI?.StartTimer = UnityEngine.Random.Range(min, max);
        AIManager.Toy_BonnieAI?.StartTimer = UnityEngine.Random.Range(min, max);
        AIManager.Toy_ChicaAI?.StartTimer = UnityEngine.Random.Range(min, max);
        AIManager.MangleAI?.StartTimer = UnityEngine.Random.Range(min, max);
        AIManager.BalloonBoyAI?.StartTimer = UnityEngine.Random.Range(min, max);
        AIManager.PuppetAI?.StartTimer = UnityEngine.Random.Range(min, max);
        AIManager.W_FreddyAI?.StartTimer = UnityEngine.Random.Range(min, max);
        AIManager.W_BonnieAI?.StartTimer = UnityEngine.Random.Range(min, max);
        AIManager.W_ChicaAI?.StartTimer = UnityEngine.Random.Range(min, max);
        AIManager.W_FoxyAI?.StartTimer = UnityEngine.Random.Range(min, max);
    }
}
