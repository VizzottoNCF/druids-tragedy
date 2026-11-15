using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCheckUI : MonoBehaviour
{
    private List<KeyCode> KeySequence = new List<KeyCode>();
    [SerializeField] private TMP_Text timer;
    [SerializeField] private Transform Frame;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject successVFX;
    [SerializeField] private GameObject failVFX;

    [Header("Keys")]
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject downArrow;
    [SerializeField] private GameObject upArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject divisor;
    [SerializeField] private List<GameObject> keys = new List<GameObject>();

    private void Start() => ShowQTEPrompt(false);
    public void Setup(List<KeyCode> keyCodes, Card c)
    {
        // clear lists and set new values
        KeySequence = keyCodes;
        keys.Clear();
        
        icon.sprite = c.Icon;
        InstantiateKeys();
    }
    public void ShowQTEPrompt(bool active)
    {
        Frame.gameObject.SetActive(active);
        timer.gameObject.SetActive(active);
        icon.gameObject.SetActive(active);
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
    public void FailQTEVisualFX()
    {
        Vector3 pos = new Vector3(0f, 2f, 0f);
        Instantiate(failVFX, pos, Quaternion.identity);
    }
    public void SuccessQTEVisualFX()
    {
        Vector3 pos = new Vector3(0f, 2f, 0f);
        Instantiate(successVFX, pos, Quaternion.identity);
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
