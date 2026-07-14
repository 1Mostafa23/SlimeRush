using UnityEngine;

public class SfxSettings
{
    public SfxSettings(
        AudioClip slimeIncreaseClip,
        AudioClip slimeCountUpgradeBoughtClip,
        AudioClip victoryRewardClip,
        float slimeIncreaseVolume,
        float uiVolume)
    {
        SlimeIncreaseClip = slimeIncreaseClip;
        SlimeCountUpgradeBoughtClip = slimeCountUpgradeBoughtClip;
        VictoryRewardClip = victoryRewardClip;
        SlimeIncreaseVolume = Mathf.Clamp01(slimeIncreaseVolume);
        UiVolume = Mathf.Clamp01(uiVolume);
    }

    public AudioClip SlimeIncreaseClip { get; }
    public AudioClip SlimeCountUpgradeBoughtClip { get; }
    public AudioClip VictoryRewardClip { get; }
    public float SlimeIncreaseVolume { get; }
    public float UiVolume { get; }
}
