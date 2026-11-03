using System.Collections.Generic;
using UnityEngine;

public class MatchData : Singleton<MatchData>
{
    public EnemyWave wave;

    [Header("Current Run Settings")]
    public List<CardData> Deck { get; set; }
    public List<PerkData> Perks { get; set; }
    public List<string> CompletedEncounters;
    public HashSet<string> RewardsClaimed { get; } = new();
    public MapData Map { get; set; }
    public string CurrentNodeID { get; set; }
    public float? PathwayCameraY { get; set; } = null;

    [Header("Settings")]
    [SerializeField] private HeroData heroData;
    public int HeroRemainingHealth { get; set; }

    private void Start()
    {
        CompletedEncounters = new List<string>();
        HeroRemainingHealth = heroData.Health;
        Perks = new List<PerkData>(heroData.Perks);
    }
    public HeroData GetHeroData() => heroData;
    public void PickNewEncounter(EnemyWave encounter)
    {
        wave = encounter;
    }
}
