using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowTip : CustomButtonScale
{
    [SerializeField] private TMP_Text tipText;
    [SerializeField] private string tooltip;

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        tipText.text = "";
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        tipText.text = tooltip;
    }

}
