using System;
using UnityEngine;
using Zenject;

public class MusicService : IMusicService, IInitializable, ITickable, IDisposable
{
    private readonly AudioClip musicClip;
    private readonly MusicSettings settings;

    private AudioSource audioSource;
    private float targetVolume;

    public MusicService(AudioClip musicClip, MusicSettings settings)
    {
        this.musicClip = musicClip;
        this.settings = settings;
        targetVolume = settings.MenuVolume;
    }

    public void Initialize()
    {
        if (musicClip == null)
            return;

        GameObject musicObject = new GameObject("MusicService");
        UnityEngine.Object.DontDestroyOnLoad(musicObject);

        audioSource = musicObject.AddComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = settings.MenuVolume;
        audioSource.Play();
    }

    public void Tick()
    {
        if (audioSource == null)
            return;

        float fadeDuration = Mathf.Max(0.01f, settings.FadeDuration);
        audioSource.volume = Mathf.MoveTowards(
            audioSource.volume,
            targetVolume,
            Time.unscaledDeltaTime / fadeDuration);
    }

    public void SetMenuMode()
    {
        targetVolume = settings.MenuVolume;
    }

    public void SetGameplayMode()
    {
        targetVolume = settings.GameplayVolume;
    }

    public void Dispose()
    {
        if (audioSource != null)
            UnityEngine.Object.Destroy(audioSource.gameObject);
    }
}
