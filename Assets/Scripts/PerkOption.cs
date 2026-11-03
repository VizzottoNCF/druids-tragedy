using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkOption : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image iconImage; // the icon, NOT the button background

    private PerkData perkData;
    private MapNode node;
    private MapView mapView;
    public void Setup(PerkData perk, MapNode shopNode, MapView view)
    {
        perkData = perk;
        node = shopNode;
        mapView = view;

        nameText.text = perk.name;
        iconImage.sprite = perk.Image;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => mapView.ChooseShopPerk(perkData, node));
    }
}