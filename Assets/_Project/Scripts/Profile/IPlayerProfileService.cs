public interface IPlayerProfileService
{
    PlayerProfileData Profile { get; }

    void Save();
}
