public readonly struct MusicSettings
{
    public MusicSettings(float menuVolume, float gameplayVolume, float fadeDuration)
    {
        MenuVolume = menuVolume;
        GameplayVolume = gameplayVolume;
        FadeDuration = fadeDuration;
    }

    public float MenuVolume { get; }
    public float GameplayVolume { get; }
    public float FadeDuration { get; }
}
