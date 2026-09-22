using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSystem : Singleton<CardSystem>
{
    private HandManager handManager;

    private readonly List<Card> drawPile = new();
    private readonly List<Card> discardPile = new();
    private readonly List<Card> hand = new();

    

    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;

    private void Start()
    {
        handManager = ServiceLocator.Get<HandManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawCardGA drawCardGA = new(1);
            ActionSystem.Instance.Perform(drawCardGA);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            DiscardAllCardsGA discardAllCardsGA = new();
            ActionSystem.Instance.Perform(discardAllCardsGA);
        }
    }

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardGA>(DrawCardPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    public void Setup(List<CardData> deckData)
    {
        foreach (var cardData in deckData)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }
    }
    public IEnumerator PlaceCardInTray(Card card, EnemyView target, GameObject view)
    {
        if (handManager == null) handManager = ServiceLocator.Get<HandManager>();

        // setups visuals
        StartCoroutine(handManager.TrayCard(view));

        // spends mana for the card
        SpendManaGA spendManaGA = new(card.Sequence);
        ActionSystem.Instance.Perform(spendManaGA);

        // adds card to the tray
        CardTraySystem.Instance.CardTray.Add(new(card, target));
        yield return null;
    }
    public IEnumerator RemoveCardFromTray(Card card, GameObject view)
    {
        if (handManager == null) handManager = ServiceLocator.Get<HandManager>();
        StartCoroutine(handManager.UntrayCard(view));
        
        SpendManaGA spendManaGA = new(-card.Sequence);
        ActionSystem.Instance.Perform(spendManaGA);

        foreach (CardTray tray in CardTraySystem.Instance.CardTray)
        {
            Debug.Log($"Tray Entry | Card: {tray.Card.Title} | " + $"Card Match: {tray.Card == card}");
        }

        CardTraySystem.Instance.CardTray.RemoveAll(c => c.Card == card);

        Debug.Log($"Tray now contains {CardTraySystem.Instance.CardTray.Count} cards");


        yield return null;
    }

    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        if (handManager == null) handManager = ServiceLocator.Get<HandManager>();

        hand.Remove(playCardGA.Card);
        //StartCoroutine(handManager.PlayCard(handManager.SelectedCard));

        //SpendManaGA spendManaGA = new(playCardGA.Card.Sequence);
        //ActionSystem.Instance.AddReaction(spendManaGA);

        //QuickTimeEventSystem.Instance.PlayedCards.Add(playCardGA.Card);

        if (playCardGA.Card.ManualEffect != null)
        {
            PerformEffectGA performEffectGA = new(playCardGA.Card.ManualEffect, new() { playCardGA.ManualTarget });
            ActionSystem.Instance.AddReaction(performEffectGA);
        }

        foreach (var effectWrapper in playCardGA.Card.OtherEffects)
        {
            List<CombatantView> targets = effectWrapper.TargetMode.GetTargets();
            PerformEffectGA performEffectGA = new(effectWrapper.Effect, targets);
            ActionSystem.Instance.AddReaction(performEffectGA);
        }

        yield return null;
    }

    private IEnumerator DrawCardPerformer(DrawCardGA drawCardsGA)
    {
        int actualAmount = Mathf.Min(drawCardsGA.Amount, drawPile.Count);
        int notDrawAmount = drawCardsGA.Amount - actualAmount;

        for (int i = 0; i < actualAmount; i++)
        {
            yield return DrawCard();
        }
        if (notDrawAmount > 0)
        {
            RefillDeck();
            for (int i = 0; i < notDrawAmount; i++)
            {
                yield return DrawCard();
            }
        }

        yield return null;
    }
    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardCardsGA)
    {
        if (handManager == null) handManager = ServiceLocator.Get<HandManager>();


        var trayCardsCopy = handManager.trayCards.ToList();
        var handCardsCopy = handManager.handCards.ToList();
        foreach (var card in handCardsCopy) // Iterate over the copied list to modify original list safely
        {
            hand.Remove(card.GetComponent<CardView>().Card);
            discardPile.Add(card.GetComponent<CardView>().Card);
            StartCoroutine(handManager.RemoveCard(card, discardPilePoint.position, discardPilePoint.rotation, true));
        }

        foreach (var card in trayCardsCopy) // Iterate over the copied list to modify original list safely
        {
            // played cards don't go to the discard pile
            hand.Remove(card.GetComponent<CardView>().Card);
            StartCoroutine(handManager.RemoveCardTray(card));
        }

        hand.Clear();
        handManager.handCards.Clear();
        handManager.trayCards.Clear();
        yield return null;
    }
    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        // discard all cards and trigger QTEs
        DiscardAllCardsGA discardAllCardsGA = new();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        // draw cards
        DrawCardGA drawCardGA = new(5);
        ActionSystem.Instance.AddReaction(drawCardGA);
    }
    private IEnumerator DrawCard()
    {
        if (handManager == null) handManager = ServiceLocator.Get<HandManager>();

        if (drawPile.Count == 0)
        {
            Debug.LogWarning("No cards in the Draw Pile");
            yield return null;
        }
        if (handManager.handCards.Count >= handManager.MaxHandSize)
        {
            Debug.LogWarning("Too many cards on hand");
            yield break;
        }

        Card card = drawPile.Draw();

        if (card != null)
        {
            // call hand manager to draw specific card
            hand.Add(card);

            StartCoroutine(handManager.AddCard(card, drawPilePoint.position, drawPilePoint.rotation));
        }
        else { Debug.LogWarning("Null card. Stopping draw"); yield break; }

        yield return null;
    }
    private void RefillDeck()
    {
        if (discardPile.Count == 0)
        {
            drawPile.Clear();
            discardPile.Clear();

            Setup(MatchSetupSystem.Instance.GetDeck());
        }
        else
        {
            drawPile.AddRange(discardPile);
            discardPile.Clear();
        }
    }
}
