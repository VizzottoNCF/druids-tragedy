using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransitionManager : MonoBehaviour
{
    [SerializeField] private ScreenFader screenFader;
    private string currentRoom = "";
    private void Start()
    {
        ServiceLocator.Register<RoomTransitionManager>(this);
        currentRoom = SceneManager.GetActiveScene().name;
    }

    public IEnumerator Transition(string sceneName)
    {
        Debug.Log(sceneName);

        // start screen transition
        if (!screenFader.isFading) { yield return screenFader.StartCoroutine(screenFader.Fade(0, 1, .25f)); }

        // unload current room and then load next
        if (!string.IsNullOrEmpty(currentRoom))
        {
            yield return SceneManager.UnloadSceneAsync(currentRoom);
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
        else { Debug.LogError("room name is null"); }

        // check if it's a valid room and change active scene
        Scene newScene = SceneManager.GetSceneByName(sceneName);
        if (newScene.IsValid()) { SceneManager.SetActiveScene(newScene); }
        
        currentRoom = SceneManager.GetActiveScene().name;

        yield return screenFader.StartCoroutine(screenFader.Fade(1, 0, .25f));
    }
}
