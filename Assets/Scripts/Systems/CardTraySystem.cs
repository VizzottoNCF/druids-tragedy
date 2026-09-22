using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public class CardTray
{
    public Card Card;
    public EnemyView ManualTarget = null;
    public GameObject View;

    public CardTray(Card card, EnemyView manualTarget = null, GameObject view = null)
    {
        Card = card;
        ManualTarget = manualTarget;
        View = view;
    }
}
public class CardTraySystem : Singleton<CardTraySystem>
{
    public List<CardTray> CardTray = new List<CardTray>();

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }
    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }
    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {

        // play all cards in tray and register them in qte
        foreach (var card in CardTray)
        {
            //// clear QTEs, and add new qte to system
            //QuickTimeEventSystem.Instance.PlayedCards.Clear();
            //QuickTimeEventSystem.Instance.PlayedCards.Add(card.Card);
            
            //// play qte
            //QuickTimeEventGA quickTimeEventGA = new(QuickTimeEventSystem.Instance.PlayedCards);
            //ActionSystem.Instance.AddReaction(quickTimeEventGA);
            
            // create play card game action
            PlayCardGA playCardGA = new(card.Card, card.ManualTarget);
            ActionSystem.Instance.AddReaction(playCardGA);
        }
    }
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        // clear tray
        CardTray.Clear();
        CardTray = new List<CardTray>();
    }
}
