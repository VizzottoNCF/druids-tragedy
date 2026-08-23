using TMPro;
using UnityEngine;

public class ManaUI : MonoBehaviour
{
    [SerializeField] private TMP_Text mana;
    public void UpdateManaText(int amount) => mana.text = amount.ToString();
}
