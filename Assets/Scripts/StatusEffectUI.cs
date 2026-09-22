using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text stackCountText;
    public void Setup(Sprite spr, int stackCount)
    {
        image.sprite = spr;
        stackCountText.text = stackCount.ToString();
    }
}
