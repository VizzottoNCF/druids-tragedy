using UnityEngine;

using System.Collections.Generic;


public class MatchSetupSystem : Singleton<MatchSetupSystem>
{
        [SerializeField] private HeroData heroData;
        [SerializeField] private List<EnemyData> enemyDatas;
    private void Start()
    {
        HeroSystem.Instance.Setup(heroData);
        EnemySystem.Instance.Setup(enemyDatas);
        CardSystem.Instance.Setup(heroData.Deck);
        DrawCardGA drawCardGA = new(5);
        ActionSystem.Instance.Perform(drawCardGA);
    }
    public List<CardData> GetDeck()
    {
        return heroData.Deck;
    }
}
