using System;
using UnityEngine;
using Zenject;

public class SfxService : ISfxService, IInitializable, IDisposable
{
    private readonly SfxSettings settings;
    private AudioSource audioSource;

    public SfxService(SfxSettings settings)
    {
        this.settings = settings;
    }

    public void Initialize()
    {
        GameObject sfxObject = new GameObject("SfxService");
        UnityEngine.Object.DontDestroyOnLoad(sfxObject);

        audioSource = sfxObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
    }

    public void PlaySlimeIncrease()
    {
        Play(settings.SlimeIncreaseClip, settings.SlimeIncreaseVolume);
    }

    public void PlaySlimeCountUpgradeBought()
    {
        Play(settings.SlimeCountUpgradeBoughtClip, settings.UiVolume);
    }

    public void PlayVictoryReward()
    {
        Play(settings.VictoryRewardClip, settings.UiVolume);
    }

    public void Dispose()
    {
        if (audioSource != null)
            UnityEngine.Object.Destroy(audioSource.gameObject);
    }

    private void Play(AudioClip clip, float volume)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip, volume);
    }
}
