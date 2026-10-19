using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Reward")]
public class RewardData : ScriptableObject
{
    public List<CardData> Cards = new();
}