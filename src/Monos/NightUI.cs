using NightmareMode.Attributes;
using NightmareMode.Data;
using NightmareMode.Enums;
using NightmareMode.Helpers;
using NightmareMode.Managers;
using NightmareMode.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NightmareMode.Monos;

/// <summary>
/// Manages the UI for individual night selection screens in the game.
/// Handles display, navigation, locking/unlocking, and loading of regular nights, challenge nights, and custom nights.
/// </summary>
internal sealed class NightUI : MonoBehaviour
{
    private static GameObject? _nightPrefab;
    private static readonly List<NightUI> _allNights = [];
    private static bool _allNightsUnlocked = false;

    private int _nightOrChallenge;
    private NightType _nightType;
    private NightsFlag _requiredNight;
    private string _title = string.Empty;
    private Sprite? _sprite;

    private TextMeshProUGUI? _completedText;
    private Button? _playButton;
    private RawImage? _rawImage;
    private GameObject? _customNightContainer;

    /// <summary>
    /// Resets the collection of all night UI instances.
    /// Called when reloading the night selection screen.
    /// </summary>
    internal static void Reset()
    {
        _allNights.Clear();
    }

    /// <summary>
    /// Sets the prefab to be used for creating night UI elements.
    /// </summary>
    /// <param name="nightPrefab">The prefab GameObject to instantiate for each night.</param>
    internal static void SetNightPrefab(GameObject nightPrefab)
    {
        _nightPrefab = nightPrefab;
    }

    /// <summary>
    /// Creates a new night UI element.
    /// </summary>
    /// <param name="title">The display title for the night.</param>
    /// <param name="nightOrChallenge">The night number or challenge ID.</param>
    /// <param name="requiredNight">The night that must be completed to unlock this night.</param>
    /// <param name="nightType">The type of night (Normal, Challenge, or Custom).</param>
    /// <param name="sprite">Optional sprite to display for the night.</param>
    /// <returns>The created NightUI component, or null if creation failed.</returns>
    internal static NightUI? Create(string title, int nightOrChallenge, NightsFlag requiredNight, NightType nightType, Sprite? sprite = null)
    {
        if (_nightPrefab == null) return null;
        var nightObj = Instantiate(_nightPrefab, _nightPrefab.transform.parent);
        Destroy(nightObj.GetComponent<NightLock>());

        nightObj.name = nightType != NightType.Challenge ? $"Night{nightOrChallenge}" : $"ChallengeNight({title})";

        var nightUI = nightObj.AddComponent<NightUI>();
        nightUI.Initialize(title, nightOrChallenge, requiredNight, nightType, sprite);

        SetupNavigation(nightObj);
        _allNights.Add(nightUI);

        foreach (var night in _allNights)
        {
            night.gameObject.SetActive(false);
        }
        _allNights.FirstOrDefault()?.gameObject.SetActive(true);

        return nightUI;
    }

    /// <summary>
    /// Initializes the night UI with the specified parameters.
    /// </summary>
    /// <param name="title">The display title for the night.</param>
    /// <param name="nightOrChallenge">The night number or challenge ID.</param>
    /// <param name="requiredNight">The night that must be completed to unlock this night.</param>
    /// <param name="nightType">The type of night.</param>
    /// <param name="sprite">Optional sprite to display.</param>
    private void Initialize(string title, int nightOrChallenge, NightsFlag requiredNight, NightType nightType, Sprite? sprite)
    {
        _title = title;
        _nightOrChallenge = nightOrChallenge;
        _requiredNight = requiredNight;
        _nightType = nightType;
        _sprite = sprite;

        SetupTitle();
        SetupCompletedText();
        SetupPlayButton();
        SetupVisuals();
        LockIfRequired();
    }

    /// <summary>
    /// Sets up the title text element for the night.
    /// Creates a new title text element from the template.
    /// </summary>
    private void SetupTitle()
    {
        var text = transform.Find("Text (TMP) (1)")?.GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            var titleTMP = Instantiate(text, transform);
            titleTMP.name = "title";
            titleTMP.fontSize = 10;
            titleTMP.SetText(_title);
            titleTMP.enableWordWrapping = false;
            titleTMP.transform.localPosition = new Vector3(0f, 20f, 0f);
            text.transform.localPosition = new Vector3(0f, 25f, 0f);
        }
    }

    /// <summary>
    /// Sets up the completed text indicator and night number display.
    /// Shows "(Completed)" if the night or challenge has been completed.
    /// </summary>
    private void SetupCompletedText()
    {
        var text = transform.Find("Text (TMP) (1)")?.GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            _completedText = Instantiate(text, text.transform.parent);
            _completedText.transform.localPosition = new Vector3(0f, 4f, 0f);
            _completedText.name = "Completed";
            _completedText.color = new Color(1f, 1f, 1f, 0.35f);
            _completedText.fontSize = 20;
            _completedText.enableWordWrapping = false;
            _completedText.SetText("");
        }

        var nightNum = transform.Find("Text (TMP) (1)")?.GetComponentInChildren<TextMeshProUGUI>(true);
        if (nightNum != null)
        {
            if (_nightType != NightType.Challenge)
            {
                nightNum.SetText($"({Translator.Get("Night")} {_nightOrChallenge})");
                nightNum.enableWordWrapping = false;

                var inight = RegisterNightAttribute.GetClassInstance(c => c.Night == _nightOrChallenge);
                if (inight != null && DataManager.CompletedNights.HasCompletedNight(inight.Night.ToNightFlag()))
                {
                    _completedText?.SetText($"<#00FF20>({Translator.Get("Night.Completed")})</color>");
                }
            }
            else
            {
                nightNum.enableWordWrapping = false;
                nightNum.SetText(Translator.Get("Night.Challenge"));

                if (RegisterChallengeAttribute.GetClassInstance(c => c.ChallengeId == _nightOrChallenge)?.Completed == true)
                {
                    _completedText?.SetText($"<#00FF20>({Translator.Get("Night.Completed")})</color>");
                }
            }
        }
    }

    /// <summary>
    /// Sets up the play button and its click handler.
    /// When clicked, hides the night UI, shows loading tips, and loads the appropriate night or challenge.
    /// </summary>
    private void SetupPlayButton()
    {
        var play = transform.Find("Play (1)");
        if (play != null)
        {
            _playButton = play.GetComponentInChildren<Button>();
            _playButton.onClick = new Button.ButtonClickedEvent();

            _playButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                CatchedSingleton<LoadingTip>.Instance.gameObject.SetActive(true);

                if (_nightType != NightType.Challenge)
                    LoadNight(_nightOrChallenge);
                else
                    LoadChallenge(_nightOrChallenge);
            });
        }
    }

    /// <summary>
    /// Sets up the visual elements of the night UI.
    /// Configures sprites, raw images, and visibility of various child elements.
    /// Handles special setup for custom nights.
    /// </summary>
    private void SetupVisuals()
    {
        if (_nightType == NightType.Challenge)
        {
            _sprite = Utils.LoadSprite("NightmareMode.Resources.Images.challenge.png", 100f);
        }

        if (_sprite != null)
        {
            var rawImage = transform.Find("RawImage")?.GetComponentInChildren<RawImage>(true);
            if (rawImage != null)
            {
                _rawImage = rawImage;
                _rawImage.texture = _sprite.texture;
                _rawImage.color = new Color(1f, 0.5f, 0.5f);
            }
        }
        else
        {
            var rawImage = transform.Find("RawImage")?.GetComponent<RawImage>();
            rawImage?.color = new Color(1f, 0.5f, 0.5f);
        }

        foreach (Transform child in gameObject.transform)
        {
            if (child.name.StartsWith("Locked"))
            {
                child.Find("Text (TMP) (3)")?.GetComponentInChildren<TextMeshProUGUI>(false).SetText(GetUnlockTip());
                continue;
            }

            if (child.name.StartsWith("Goals"))
            {
                child.gameObject.SetActive(false);
                continue;
            }

            if (child.name.StartsWith("Play"))
            {
                child.gameObject.SetActive(true);
                continue;
            }
        }

        if (_nightType == NightType.CustomNight)
        {
            _playButton?.transform.localPosition = new Vector3(0f, -16.5f, 0f);

            var rawImage1 = transform.Find("RawImage");
            var rawImage2 = transform.Find("RawImage (1)");

            if (rawImage1 != null) Destroy(rawImage1.gameObject);
            if (rawImage2 != null) Destroy(rawImage2.gameObject);

            CustomNightManager.LoadCustomNightMenu(this);
        }
    }

    /// <summary>
    /// Locks or unlocks the night based on completion requirements.
    /// Shows/hides the locked overlay and play button accordingly.
    /// </summary>
    internal void LockIfRequired()
    {
        if (_requiredNight != NightsFlag.None && !DataManager.CompletedNights.HasCompletedNight(_requiredNight) && !_allNightsUnlocked)
        {
            foreach (Transform child in gameObject.transform)
            {
                if (child.name.StartsWith("Locked"))
                {
                    child.gameObject.SetActive(true);
                }
                if (child.name == "Characters")
                {
                    child.gameObject.SetActive(false);
                }
            }

            _playButton?.gameObject.SetActive(false);
        }
        else
        {
            foreach (Transform child in gameObject.transform)
            {
                if (child.name.StartsWith("Locked"))
                {
                    child.gameObject.SetActive(false);
                }
                if (child.name == "Characters")
                {
                    child.gameObject.SetActive(true);
                }
            }

            _playButton?.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Sets up the navigation buttons (Back/Forward) for cycling through nights.
    /// Creates a circular navigation system where the last night connects back to the first.
    /// </summary>
    /// <param name="nightObj">The night GameObject to set up navigation for.</param>
    private static void SetupNavigation(GameObject nightObj)
    {
        if (nightObj == null) return;
        if (_allNights.Count == 0) return;

        var firstNight = _allNights.FirstOrDefault();
        var lastNight = _allNights.LastOrDefault();

        var lastNightNextButton = lastNight?.transform.Find("Forward")?.GetComponentInChildren<Button>(true);
        if (lastNightNextButton == null) return;

        lastNightNextButton.onClick = new Button.ButtonClickedEvent();
        lastNightNextButton.onClick.AddListener(() =>
        {
            lastNight?.gameObject.SetActive(false);
            nightObj.SetActive(true);
        });

        var newNightBackButton = nightObj.transform.Find("Back")?.GetComponentInChildren<Button>(true);
        if (newNightBackButton != null)
        {
            newNightBackButton.onClick = new Button.ButtonClickedEvent();
            newNightBackButton.onClick.AddListener(() =>
            {
                nightObj.SetActive(false);
                lastNight?.gameObject.SetActive(true);
            });
        }

        var newNightForwardButton = nightObj.transform.Find("Forward")?.GetComponentInChildren<Button>(true);
        if (newNightForwardButton != null)
        {
            newNightForwardButton.onClick = new Button.ButtonClickedEvent();
            newNightForwardButton.onClick.AddListener(() =>
            {
                nightObj.SetActive(false);
                firstNight?.gameObject.SetActive(true);
            });
        }

        if (_allNights.Count > 0)
        {
            var firstNightBackButton = firstNight?.transform.Find("Back")?.GetComponentInChildren<Button>(true);

            if (firstNightBackButton != null)
            {
                firstNightBackButton.onClick = new Button.ButtonClickedEvent();
                firstNightBackButton.onClick.AddListener(() =>
                {
                    firstNight?.gameObject.SetActive(false);
                    nightObj.SetActive(true);
                });
            }
        }
    }

    /// <summary>
    /// Gets the unlock tip text to display when a night is locked.
    /// </summary>
    /// <returns>A localized string explaining how to unlock this night.</returns>
    private string GetUnlockTip()
    {
        switch (_nightType)
        {
            case NightType.Night:
                return Translator.Get("Night.UnlockTip", $"{_nightOrChallenge - 1}");
            default:
                return Translator.Get("Night.UnlockTip", "6");
        }
    }

    /// <summary>
    /// Loads a regular night by its number.
    /// </summary>
    /// <param name="night">The night number to load (1-6).</param>
    internal static void LoadNight(int night)
    {
        if (night == 1)
        {
            PlayerPrefs.SetInt("ending", 0);
        }

        if (night < 7)
        {
            PlayerPrefs.SetInt("night", night);
        }
        BrainScript.night = night;
        GameObject.Find("cinematic").SetActive(false);
        SceneManager.LoadScene("Nights");
    }

    /// <summary>
    /// Loads a challenge night by its ID.
    /// </summary>
    /// <param name="challengeId">The challenge ID to load.</param>
    internal static void LoadChallenge(int challengeId)
    {
        NightManager.IsChallengeNight = true;
        NightManager.CurrentChallengeId = challengeId;
        BrainScript.night = 8;
        GameObject.Find("cinematic").SetActive(false);
        SceneManager.LoadScene("Nights");
    }

    /// <summary>
    /// Unlocks all nights regardless of completion requirements.
    /// </summary>
    internal static void UnloadAllNights()
    {
        if (_allNightsUnlocked) return;
        _allNightsUnlocked = true;
        foreach (var night in _allNights)
        {
            night.LockIfRequired();
        }
    }
}