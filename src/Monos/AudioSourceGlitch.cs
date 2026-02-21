using UnityEngine;

namespace NightmareMode.Monos;

/// <summary>
/// Applies glitch effects to an AudioSource, creating distortion and instability in audio playback.
/// Can simulate audio corruption through volume fluctuations, pitch variations, and dramatic pitch drops.
/// </summary>
internal sealed class AudioSourceGlitch : MonoBehaviour
{
    private AudioSource? audioSource;
    private float baseVolume;
    private float basePitch;

    /// <summary>
    /// Intensity of volume glitching effects. Higher values create more dramatic volume fluctuations.
    /// </summary>
    internal float volumeGlitchIntensity = 0.3f;

    /// <summary>
    /// Intensity of pitch glitching effects. Higher values create more dramatic pitch variations.
    /// </summary>
    internal float pitchGlitchIntensity = 0.5f;

    /// <summary>
    /// Speed at which glitch patterns evolve. Higher values create faster, more erratic glitching.
    /// </summary>
    internal float glitchSpeed = 5f;

    /// <summary>
    /// Chance (0-1) of an extreme glitch occurring that doubles the pitch variation intensity.
    /// </summary>
    internal float extremeGlitchChance = 0.1f;

    private float pitchDropSpeed = 0.5f;
    private float pitchDropTarget = 0f;

    private float[] offsets = new float[3];
    private float[] speeds = new float[3];

    /// <summary>
    /// Controls whether glitch effects are currently active.
    /// </summary>
    internal bool doGlitch = true;

    private bool isPitchDropping = false;
    private bool audioDisabledAfterDrop = false;

    /// <summary>
    /// Initializes the glitch component by finding the AudioSource and setting up glitch patterns.
    /// Destroys itself if the GameObject doesn't have exactly one AudioSource component.
    /// </summary>
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

    /// <summary>
    /// Initializes random offsets and speeds for the three glitch waveforms.
    /// Each waveform operates at slightly different speeds to create complex interference patterns.
    /// </summary>
    private void InitializeGlitchPatterns()
    {
        for (int i = 0; i < 3; i++)
        {
            offsets[i] = UnityEngine.Random.Range(0f, 100f);
            speeds[i] = UnityEngine.Random.Range(0.8f, 1.2f) * glitchSpeed;
        }
    }

    /// <summary>
    /// Updates glitch effects each frame. Handles normal glitching or pitch drop sequences.
    /// </summary>
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

    /// <summary>
    /// Applies real-time glitch effects to volume and pitch using combined sine waves.
    /// Creates natural-sounding audio corruption through waveform interference.
    /// </summary>
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

    /// <summary>
    /// Handles the pitch drop sequence, gradually lowering pitch to the target value
    /// and disabling the audio source once the target is reached.
    /// </summary>
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

    /// <summary>
    /// Triggers a dramatic pitch drop effect that gradually lowers the audio pitch to zero,
    /// then disables the audio source. Useful for simulating audio system failure.
    /// </summary>
    internal void TriggerPitchDropAndDisable()
    {
        if (audioSource != null && !isPitchDropping)
        {
            isPitchDropping = true;
            audioDisabledAfterDrop = false;
        }
    }

    /// <summary>
    /// Resets the audio source to its base state, restoring normal volume and pitch,
    /// and ending any active pitch drop sequences.
    /// </summary>
    internal void ResetAudioSource()
    {
        if (audioSource == null) return;

        isPitchDropping = false;
        audioDisabledAfterDrop = false;
        audioSource.pitch = basePitch;
        audioSource.volume = baseVolume;
    }
}