public interface IPlayerProfileStorage
{
    bool TryLoad(out PlayerProfileData profileData);
    void Save(PlayerProfileData profileData);
}
