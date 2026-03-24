using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        LoadSceneIfNotLoaded("World");
        LoadSceneIfNotLoaded("Player");
        LoadSceneIfNotLoaded("UI");
    }

    void LoadSceneIfNotLoaded(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.Log($"Scene {sceneName} already loaded, skipping loading.");
            return;
        }
        else
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");

        string trackerContent = GetComponent<Tracker>().Dump();
        string achievementContent = GetComponent<AchievementManager>().Dump();

        string currentTimeString = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        TryWrite(Application.dataPath, currentTimeString, "tracking_data.csv", trackerContent);
        TryWrite(Application.persistentDataPath, currentTimeString, "tracking_data.csv", trackerContent);
        TryWrite(Application.dataPath, currentTimeString, "achievement_data.csv", achievementContent);
        TryWrite(Application.persistentDataPath, currentTimeString, "achievement_data.csv", achievementContent);
    }

    private void TryWrite(string rootPath, string currentTimeString, string fileName, string data)
    {
        string filePath = Path.Combine(rootPath, "Analytics", "data", currentTimeString, fileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, data);
            Debug.Log($"Tracker data saved to {filePath}:1");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed writing tracker data to {filePath}:1 : {e}");
        }
    }
}

// Allow record to be used
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
