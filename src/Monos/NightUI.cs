using NightmareMode.Data;
using NightmareMode.Helpers;
using NightmareMode.Items.Attributes;
using NightmareMode.Items.Enums;
using NightmareMode.Managers;
using NightmareMode.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NightmareMode.Monos;

internal class NightUI : MonoBehaviour
{
    internal static GameObject? NightPrefab;

    internal static readonly List<NightUI> _allNights = [];
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

    internal static NightUI? Create(string title, int nightOrChallenge, NightsFlag requiredNight, NightType nightType, Sprite? sprite = null)
    {
        if (NightPrefab == null) return null;
        var nightObj = Instantiate(NightPrefab, NightPrefab.transform.parent);
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
                nightNum.SetText($"(Night {_nightOrChallenge})");
                nightNum.enableWordWrapping = false;

                var inight = RegisterNightAttribute.GetClassInstance(c => c.Night == _nightOrChallenge);
                if (inight != null && DataManager.CompletedNights.HasCompletedNight(inight.Night.ToNightFlag()))
                {
                    _completedText?.SetText("<#00FF20>(Completed)</color>");
                }
            }
            else
            {
                nightNum.enableWordWrapping = false;
                nightNum.SetText($"(Challenge Night)");

                if (RegisterChallengeAttribute.GetClassInstance(c => c.ChallengeId == _nightOrChallenge)?.Completed == true)
                {
                    _completedText?.SetText("<#00FF20>(Completed)</color>");
                }
            }
        }
    }

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
            _playButton?.transform.localPosition = new Vector3(0f, -15f, 0f);

            var rawImage1 = transform.Find("RawImage");
            var rawImage2 = transform.Find("RawImage (1)");

            if (rawImage1 != null) Destroy(rawImage1.gameObject);
            if (rawImage2 != null) Destroy(rawImage2.gameObject);

            CustomNightManager.LoadMenu(this);
        }
    }

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

    private string GetUnlockTip()
    {
        switch (_nightType)
        {
            case NightType.Night:
                return $"Complete Night {_nightOrChallenge - 1} to unlock.";
            default:
                return "Complete Night 6 to unlock.";
        }
    }

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

    internal static void LoadChallenge(int challengeId)
    {
        NightManager.IsChallengeNight = true;
        NightManager.CurrentChallengeId = challengeId;
        BrainScript.night = 8;
        GameObject.Find("cinematic").SetActive(false);
        SceneManager.LoadScene("Nights");
    }

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