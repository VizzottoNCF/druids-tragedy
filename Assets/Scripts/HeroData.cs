using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Hero")]
public class HeroData : ScriptableObject
{
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public int Sequence { get; private set; }
    [field: SerializeField] public int TraySize { get; private set; }
    [field: SerializeField] public List<PerkData> Perks { get; private set; }
    [field: SerializeField] public List<CardData> Deck { get; private set; }
}
