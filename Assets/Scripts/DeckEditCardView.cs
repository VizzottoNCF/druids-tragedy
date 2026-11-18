using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class DeckEditCardView : CustomButtonScale
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text sequence;
    [SerializeField] private Image art;
    [SerializeField] private Image background;

    public CardData Data { get; private set; }
    public int Index { get; private set; }
    public bool Locked { get; private set; }

    private bool isSelected;
    private Action<DeckEditCardView> onClick;
    

    public void Setup(CardData data, int index, Action<DeckEditCardView> clicked)
    {
        Data = data;
        Index = index;
        onClick = clicked;
        isSelected = false;
        Locked = false;

        title.text = data.Title;
        description.text = data.Description;
        sequence.text = data.Sequence.ToString();
        if (art != null) art.sprite = data.Icon;
        UpdateVisual();
    }

    public void SetSelected(bool selected) { isSelected = selected; UpdateVisual(); }
    public void SetLocked(bool locked) { Locked = locked; UpdateVisual(); }

    private void UpdateVisual()
    {
        background.color = Locked ? Color.gray : isSelected ? Color.rosyBrown : Color.brown;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (!Locked) onClick?.Invoke(this);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        DeckEditSystem.Instance.ShowCardUI(Data);
    }
}