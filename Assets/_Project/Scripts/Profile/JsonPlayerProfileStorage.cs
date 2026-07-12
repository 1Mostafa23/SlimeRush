using System;
using System.IO;
using UnityEngine;

public class JsonPlayerProfileStorage : IPlayerProfileStorage
{
    private const string SaveFileName = "player_profile.json";

    private readonly string savePath;

    public JsonPlayerProfileStorage()
    {
        savePath = Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    public bool TryLoad(out PlayerProfileData profileData)
    {
        profileData = null;

        if (!File.Exists(savePath))
            return false;

        try
        {
            string json = File.ReadAllText(savePath);
            profileData = JsonUtility.FromJson<PlayerProfileData>(json);
            return profileData != null;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"JsonPlayerProfileStorage: Failed to load profile at '{savePath}'. {exception.Message}");
            return false;
        }
    }

    public void Save(PlayerProfileData profileData)
    {
        if (profileData == null)
            return;

        string directory = Path.GetDirectoryName(savePath);

        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        string json = JsonUtility.ToJson(profileData, true);
        File.WriteAllText(savePath, json);
    }
}
