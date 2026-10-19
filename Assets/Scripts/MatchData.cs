using System.Collections.Generic;
using UnityEngine;

public class MatchData : Singleton<MatchData>
{
    public EnemyWave wave;

    [Header("Current Run Settings")]
    public List<CardData> Deck { get; set; }
    public List<string> CompletedEncounters;
    public HashSet<string> RewardsClaimed { get; } = new();
    public MapData Map { get; set; }
    public string CurrentNodeID { get; set; }

    [Header("Settings")]
    [SerializeField] private HeroData heroData;
    public int HeroRemainingHealth { get; set; }

    private void Start()
    {
        CompletedEncounters = new List<string>();
        HeroRemainingHealth = heroData.Health;
    }

    public void PickNewEncounter(EnemyWave encounter)
    {
        wave = encounter;
    }
}
