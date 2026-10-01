using UnityEngine;

public class LoadMenu : MonoBehaviour
{
    public void Load()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
