using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    public bool CanPause { get; set; } = true;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && CanPause)
        {
            _panel.SetActive(!_panel.activeSelf);
            Time.timeScale = _panel.activeSelf ? 0 : 1;
        }
    }
}
