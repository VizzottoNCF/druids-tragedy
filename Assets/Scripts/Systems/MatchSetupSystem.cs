using UnityEngine;
using System.Collections.Generic;
using System;
public class MatchSetupSystem : Singleton<MatchSetupSystem>
{
    [SerializeField] private HeroData heroData;
    [SerializeField] private PerkData perkData;
    private void Start()
    {
        HeroSystem.Instance.Setup(heroData);
        EnemySystem.Instance.Setup(MatchData.Instance.wave);
        CardSystem.Instance.Setup(heroData.Deck);
        PerkSystem.Instance.AddPerk(new Perk(perkData));
        DrawCardGA drawCardGA = new(5);
        ActionSystem.Instance.Perform(drawCardGA);
    }
    public List<CardData> GetDeck()
    {
        return heroData.Deck;
    }
}
[Serializable]
[CreateAssetMenu(menuName = "EnemyWave")]
public class EnemyWave : ScriptableObject
{
    public List<EnemyData> enemyDatas;
}