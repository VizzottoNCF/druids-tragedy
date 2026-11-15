using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkOption : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage; // the icon, NOT the button background

    private PerkData perkData;
    private MapNode node;
    private MapView mapView;
    public void Setup(PerkData perk, MapNode shopNode, MapView view)
    {
        perkData = perk;
        node = shopNode;
        mapView = view;

        nameText.text = perk.Title;
        descriptionText.text = perk.Description;
        iconImage.sprite = perk.Image;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => mapView.ChooseShopPerk(perkData, node));
    }
}