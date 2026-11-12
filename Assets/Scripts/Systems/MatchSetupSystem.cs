using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
public class MatchSetupSystem : Singleton<MatchSetupSystem>
{
    [SerializeField] private HeroData heroData;
    public string encounterID = string.Empty;
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<StartMatchGA>(StartMatchPerformer);
        ActionSystem.AttachPerformer<StartTutorialMatchGA>(StartTutorialMatchPerformer);
        ActionSystem.AttachPerformer<EndMatchGA>(EndMatchPerformer);
        ActionSystem.SubscribeReaction<StartMatchGA>(StartMatchPostReaction, ReactionTiming.POST);
        ActionSystem.SubscribeReaction<StartTutorialMatchGA>(StartTutorialMatchPostReaction, ReactionTiming.POST);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<StartMatchGA>();
        ActionSystem.DetachPerformer<StartTutorialMatchGA>();
        ActionSystem.DetachPerformer<EndMatchGA>();
        ActionSystem.UnsubscribeReaction<StartMatchGA>(StartMatchPostReaction, ReactionTiming.POST);
        ActionSystem.UnsubscribeReaction<StartTutorialMatchGA>(StartTutorialMatchPostReaction, ReactionTiming.POST);
    }
    public List<CardData> GetDeck()
    {
        MatchData.Instance.Deck ??= new List<CardData>(heroData.Deck);
        return MatchData.Instance.Deck;
    }

    private IEnumerator StartTutorialMatchPerformer(StartTutorialMatchGA startMatchGA)
    {
        RoomTransitionManager service = ServiceLocator.Get<RoomTransitionManager>();
        Debug.Log("Starting tutorial match, transitioning to tutorial scene");
        yield return (StartCoroutine(service.Transition("Tutorial")));
    }
    private IEnumerator StartMatchPerformer(StartMatchGA startMatchGA)
    {
        // update encounter ID to current combat
        encounterID = startMatchGA.EncounterID;

        // change to combat scene
        RoomTransitionManager service = ServiceLocator.Get<RoomTransitionManager>();
        yield return (StartCoroutine(service.Transition("Combat")));
    }
    private void StartTutorialMatchPostReaction(StartTutorialMatchGA startMatchGA)
    {
        // setup for tutorial scene
        Debug.Log("StartMatchPostReaction");
        // tutorial
        if (TutorialSystem.Instance != null) TutorialSystem.Instance.Begin();
        if (TutorialSystem.Active)
        {
            Debug.Log("Tutorial active, setting up tutorial data");
            PerkSystem.Instance.Setup(TutorialSystem.Instance.data.Perks);
            CardSystem.Instance.Setup(TutorialSystem.Instance.data.FixedHand);
            EnemySystem.Instance.Setup(TutorialSystem.Instance.data.Wave);

        }

        HeroSystem.Instance.Setup(heroData);        
        HandManager hm = ServiceLocator.Get<HandManager>();
        hm.Setup(heroData.TraySize);
        DrawCardGA drawCardGA = new(4);
        ActionSystem.Instance.AddReaction(drawCardGA);
        ManaSystem.Instance.Setup(heroData.Sequence);
    }
    private void StartMatchPostReaction(StartMatchGA startMatchGA)
    {
        // setup for combat scene
        PerkSystem.Instance.Setup(MatchData.Instance.Perks);
        CardSystem.Instance.Setup(GetDeck());


        HeroSystem.Instance.Setup(heroData);
        EnemySystem.Instance.Setup(MatchData.Instance.wave);
        HandManager hm = ServiceLocator.Get<HandManager>();
        hm.Setup(heroData.TraySize);
        DrawCardGA drawCardGA = new(4);
        ActionSystem.Instance.AddReaction(drawCardGA);
        ManaSystem.Instance.Setup(heroData.Sequence);


    }
    private IEnumerator EndMatchPerformer(EndMatchGA endMatchGA)
    {
        if (TutorialSystem.Active)
        {
            // change back to main menu scene
            RoomTransitionManager service = ServiceLocator.Get<RoomTransitionManager>();
            yield return (StartCoroutine(service.Transition("Menu")));
            yield return null;
        }
        else
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
}