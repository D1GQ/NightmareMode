using UnityEngine;

namespace NightmareMode.Monos;

internal sealed class AudioSourceGlitch : MonoBehaviour
{
    private AudioSource? audioSource;
    private float baseVolume;
    private float basePitch;

    internal float volumeGlitchIntensity = 0.3f;
    internal float pitchGlitchIntensity = 0.5f;
    internal float glitchSpeed = 5f;
    internal float extremeGlitchChance = 0.1f;

    private float pitchDropSpeed = 0.5f;
    private float pitchDropTarget = 0f;

    private float[] offsets = new float[3];
    private float[] speeds = new float[3];

    internal bool doGlitch = true;
    private bool isPitchDropping = false;
    private bool audioDisabledAfterDrop = false;

    private void Start()
    {
        var audios = GetComponentsInChildren<AudioSource>(true);

        if (audios.Length != 1)
        {
            Destroy(this);
            return;
        }

        audioSource = audios[0];
        baseVolume = audioSource.volume;
        basePitch = audioSource.pitch;

        InitializeGlitchPatterns();
    }

    private void InitializeGlitchPatterns()
    {
        for (int i = 0; i < 3; i++)
        {
            offsets[i] = UnityEngine.Random.Range(0f, 100f);
            speeds[i] = UnityEngine.Random.Range(0.8f, 1.2f) * glitchSpeed;
        }
    }

    private void Update()
    {
        if (audioSource == null) return;

        if (isPitchDropping)
        {
            HandlePitchDrop();
            return;
        }

        if (!doGlitch) return;

        ApplyGlitchEffects();
    }

    private void ApplyGlitchEffects()
    {
        if (audioSource == null) return;
        float time = Time.time;
        float volumeMod = 0f;
        float pitchMod = 0f;

        for (int i = 0; i < 3; i++)
        {
            float t = offsets[i] + time * speeds[i];
            float wave = Mathf.Sin(t) + Mathf.Sin(t * 0.73f) + Mathf.Sin(t * 1.37f);

            if (i < 2) volumeMod += wave * 0.15f;
            if (i > 0) pitchMod += wave * 0.15f;
        }

        audioSource.volume = baseVolume * Mathf.Clamp(1f + volumeMod * volumeGlitchIntensity, 0f, 2f);

        float pitchMultiplier = 1f;
        if (UnityEngine.Random.value < extremeGlitchChance)
        {
            pitchMultiplier += UnityEngine.Random.Range(-0.5f, 0.5f) * pitchGlitchIntensity * 2f;
        }

        audioSource.pitch = basePitch * Mathf.Clamp(1f + pitchMod * pitchGlitchIntensity * pitchMultiplier, 0.1f, 3f);
    }

    private void HandlePitchDrop()
    {
        if (audioSource == null) return;
        if (audioSource.pitch > pitchDropTarget)
        {
            audioSource.pitch = Mathf.MoveTowards(
                audioSource.pitch,
                pitchDropTarget,
                pitchDropSpeed * Time.deltaTime
            );
        }
        else if (!audioDisabledAfterDrop)
        {
            audioSource.Stop();
            audioSource.enabled = false;
            audioDisabledAfterDrop = true;
            ResetAudioSource();
        }
    }

    internal void TriggerPitchDropAndDisable()
    {
        if (audioSource != null && !isPitchDropping)
        {
            isPitchDropping = true;
            audioDisabledAfterDrop = false;
        }
    }

    internal void ResetAudioSource()
    {
        if (audioSource == null) return;

        isPitchDropping = false;
        audioDisabledAfterDrop = false;
        audioSource.pitch = basePitch;
        audioSource.volume = baseVolume;
    }
}