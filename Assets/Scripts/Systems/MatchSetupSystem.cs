using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
public class MatchSetupSystem : Singleton<MatchSetupSystem>
{
    [SerializeField] private HeroData heroData;
    [SerializeField] private PerkData perkData;
    public string encounterID = string.Empty;
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<StartMatchGA>(StartMatchPerformer);
        ActionSystem.AttachPerformer<EndMatchGA>(EndMatchPerformer);
        ActionSystem.SubscribeReaction<StartMatchGA>(StartMatchPostReaction, ReactionTiming.POST);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<StartMatchGA>();
        ActionSystem.DetachPerformer<EndMatchGA>();
        ActionSystem.UnsubscribeReaction<StartMatchGA>(StartMatchPostReaction, ReactionTiming.POST);
    }
    private void Start()
    {
        
    }
    public List<CardData> GetDeck()
    {
        MatchData.Instance.Deck ??= new List<CardData>(heroData.Deck);
        return MatchData.Instance.Deck;
    }
    private IEnumerator StartMatchPerformer(StartMatchGA startMatchGA)
    {
        // update encounter ID to current combat
        encounterID = startMatchGA.EncounterID;


        // change to combat scene
        RoomTransitionManager service = ServiceLocator.Get<RoomTransitionManager>();
        yield return (StartCoroutine(service.Transition("Combat")));
    }
    private void StartMatchPostReaction(StartMatchGA startMatchGA)
    {
        // setup combat
        HeroSystem.Instance.Setup(heroData);
        EnemySystem.Instance.Setup(MatchData.Instance.wave);
        CardSystem.Instance.Setup(GetDeck());
        PerkSystem.Instance.AddPerk(new Perk(perkData));
        DrawCardGA drawCardGA = new(5);
        ActionSystem.Instance.AddReaction(drawCardGA);
    }
    private IEnumerator EndMatchPerformer(EndMatchGA endMatchGA)
    {
        // add encounter ID as finished 
        MatchData.Instance.CompletedEncounters.Add(endMatchGA.EncounterID);

        // save remaining health
        MatchData.Instance.HeroRemainingHealth = HeroSystem.Instance.heroView.GetCurrentHealth();

        // change back to pathway scene
        RoomTransitionManager service = ServiceLocator.Get<RoomTransitionManager>();
        yield return (StartCoroutine(service.Transition("Pathway")));

        yield return null;
    }
}