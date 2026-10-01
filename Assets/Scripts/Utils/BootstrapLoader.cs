using UnityEngine;
using UnityEngine.SceneManagement;

public static class BootstrapLoader
{
    private const string BootstrapScene = "Bootstrap";
    private const string MainMenuScene = "Menu";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureBootstrapLoaded(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureBootstrapLoaded(scene);
    }

    // bootstrap is an omnipresent scene that holds vital data that need to be mantained and is vital to most scenes
    private static void EnsureBootstrapLoaded(Scene scene)
    {
        if (scene.name == MainMenuScene)
        {
            //AudioManager.Instance.StopAllSongs();

            if (SceneManager.GetSceneByName(BootstrapScene).isLoaded)
            {
                SceneManager.UnloadSceneAsync(BootstrapScene);
            }
            return;
        }


        if (!SceneManager.GetSceneByName(BootstrapScene).isLoaded)
        {
            SceneManager.LoadSceneAsync(BootstrapScene, LoadSceneMode.Additive);
        }

    }
}
