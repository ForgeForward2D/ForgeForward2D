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
            Debug.Log("Scene " + sceneName + " already loaded, skipping loading.");
            return;
        } else {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }
}

// Allow record to be used
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}
