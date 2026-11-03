using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _panel.SetActive(!_panel.activeSelf);
            Time.timeScale = _panel.activeSelf ? 0 : 1;
        }
    }
}
