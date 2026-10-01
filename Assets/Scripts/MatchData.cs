using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MatchData : Singleton<MatchData>
{
    public EnemyWave wave;

    [Header("Current Run Settings")]
    public List<CardData> Deck { get; set; }
    public List<string> CompletedEncounters;
    public HashSet<string> RewardsClaimed { get; } = new();

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
