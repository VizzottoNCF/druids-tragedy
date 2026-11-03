using UnityEngine;

public class LoadMenu : MonoBehaviour
{
    public void Load()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        Time.timeScale = 1f; // Reset time scale to normal
    }
}
