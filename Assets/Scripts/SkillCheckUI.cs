using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCheckUI : MonoBehaviour
{
    private List<KeyCode> KeySequence = new List<KeyCode>();
    [SerializeField] private TMP_Text timer;
    [SerializeField] private Transform Frame;

    [Header("Keys")]
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject downArrow;
    [SerializeField] private GameObject upArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject divisor;
    [SerializeField] private List<GameObject> keys = new List<GameObject>();

    private void Start() => ShowQTEPrompt(false);
    public void Setup(List<KeyCode> keyCodes)
    {
        // clear lists and set new values
        KeySequence = keyCodes;
        keys.Clear();

        InstantiateKeys();
    }
    public void ShowQTEPrompt(bool active)
    {
        Frame.gameObject.SetActive(active);
        timer.gameObject.SetActive(active);
    }
    public void ClearQTE()
    {
        foreach (GameObject k in keys)
        {
            Destroy(k);
        }

        keys.Clear();
        KeySequence.Clear();
    }
    private void InstantiateKeys()
    {
        foreach (KeyCode key in KeySequence)
        {
            GameObject k = null;
            switch (key)
            {
                case KeyCode.UpArrow:
                    k = Instantiate(upArrow, Frame);
                    break;
                case KeyCode.DownArrow:
                    k = Instantiate(downArrow, Frame);
                    break;
                case KeyCode.LeftArrow:
                    k = Instantiate(leftArrow, Frame);
                    break;
                case KeyCode.RightArrow:
                    k = Instantiate(rightArrow, Frame);
                    break;
                case KeyCode.Space:
                    k = Instantiate(divisor, Frame);
                    break;
                default:
                    Debug.LogWarning($"Unhandled key: {key}");
                    break;
            }
            if (k != null) keys.Add(k);
        }
    }
    public void ChangeKeyColor(int index, Color c, bool all = false)
    {
        if (all)
        {
            foreach (var key in keys)
            {
                key.GetComponent<Image>().color = c;
            }
        }

        else
        {
            keys[index].GetComponent<Image>().color = c;
        }
    }
    public void UpdateTimer(float time)
    {
        timer.text = time.ToString("F1");
    }
}
