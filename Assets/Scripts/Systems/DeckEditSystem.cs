using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckEditSystem : Singleton<DeckEditSystem>
{
    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private DeckEditCardView cardViewPrefab;
    [SerializeField] private Transform rewardsContainer;
    [SerializeField] private Transform deckContainer;
    [SerializeField] private TMP_Text changesText;
    [SerializeField] private Button addButton;
    [SerializeField] private Button removeButton;

    [Header("Config")]
    [SerializeField] private int maxChanges = 5;
    [SerializeField] private int minDeckSize = 8;

    public bool IsEditing { get; private set; }

    private List<CardData> workingDeck;
    private readonly List<DeckEditCardView> deckViews = new();
    private readonly List<DeckEditCardView> rewardViews = new();
    private DeckEditCardView selectedReward, selectedDeckCard;
    private int changesMade, changesAllowed;

    protected override void Awake()
    {
        base.Awake();
        panel.SetActive(false);
    }

    public void StartEdit(List<CardData> rewards)
    {
        if (IsEditing) return;
        IsEditing = true;

        // makes a copy and works on it, original deck is only modified upon confirmation
        workingDeck = new List<CardData>(MatchSetupSystem.Instance.GetDeck());
        changesMade = 0;
        changesAllowed = Mathf.Min(maxChanges, rewards.Count);
        selectedReward = selectedDeckCard = null;

        ClearViews();
        for (int i = 0; i < workingDeck.Count; i++)
            deckViews.Add(SpawnView(workingDeck[i], i, deckContainer, OnDeckCardClicked));
        for (int i = 0; i < rewards.Count; i++)
            rewardViews.Add(SpawnView(rewards[i], i, rewardsContainer, OnRewardClicked));

        UpdateUI();
        panel.SetActive(true);
    }

    private DeckEditCardView SpawnView(CardData data, int index, Transform parent, Action<DeckEditCardView> onClick)
    {
        DeckEditCardView view = Instantiate(cardViewPrefab, parent);
        view.Setup(data, index, onClick);
        return view;
    }
    private bool HasChangesLeft => changesMade < changesAllowed;
    private bool CanRemove => workingDeck != null && workingDeck.Count > minDeckSize;
    private void OnRewardClicked(DeckEditCardView view)
    {
        if (!HasChangesLeft) return;

        if (selectedReward == view) // double click deselects it
        {
            view.SetSelected(false);
            selectedReward = null;
        }
        else
        {
            selectedReward?.SetSelected(false);
            selectedReward = view;
            view.SetSelected(true);
            TrySwap();
        }
        UpdateUI();
    }
    private void OnDeckCardClicked(DeckEditCardView view)
    {
        if (!HasChangesLeft) return;

        if (selectedDeckCard == view)
        {
            view.SetSelected(false);
            selectedDeckCard = null;
        }
        else
        {
            selectedDeckCard?.SetSelected(false);
            selectedDeckCard = view;
            view.SetSelected(true);
            TrySwap();
        }
        UpdateUI();
    }

    private void TrySwap()
    {
        if (selectedReward == null || selectedDeckCard == null) return;

        CardData newCard = selectedReward.Data;
        int slot = deckViews.IndexOf(selectedDeckCard);
        workingDeck[slot] = newCard;

        // swaps card view and locks it
        selectedDeckCard.Setup(newCard, slot, OnDeckCardClicked);
        selectedDeckCard.SetLocked(true);

        ConsumeSelectedReward();
    }

    // connect to OnClick of add to deck button in UI
    public void AddSelectedRewardToDeck()
    {
        if (!IsEditing || !HasChangesLeft || selectedReward == null) return;

        CardData newCard = selectedReward.Data;
        workingDeck.Add(newCard);

        // spawns new card view in the deck and locks it
        DeckEditCardView view = SpawnView(newCard, workingDeck.Count - 1, deckContainer, OnDeckCardClicked);
        view.SetLocked(true);
        deckViews.Add(view);

        // deselect cards
        if (selectedDeckCard != null)
        {
            selectedDeckCard.SetSelected(false);
            selectedDeckCard = null;
        }

        ConsumeSelectedReward();
        UpdateUI();
    }
    private void ConsumeSelectedReward()
    {
        rewardViews.Remove(selectedReward);
        Destroy(selectedReward.gameObject);

        selectedReward = null;
        selectedDeckCard = null;
        changesMade++;
        UpdateUI();
    }
    // connect to OnClick of remove from deck button in UI
    public void RemoveSelectedDeckCard()
    {
        if (!IsEditing || !HasChangesLeft || !CanRemove || selectedDeckCard == null) return;

        int slot = deckViews.IndexOf(selectedDeckCard);
        workingDeck.RemoveAt(slot);
        deckViews.RemoveAt(slot);
        Destroy(selectedDeckCard.gameObject);

        // deselect cards
        if (selectedDeckCard != null)
        {
            selectedDeckCard.SetSelected(false);
            selectedDeckCard = null;
        }

        selectedDeckCard = null;
        selectedReward = null;
        changesMade++;
        UpdateUI();
    }

    // connect to OnClick of confirm button in UI
    public void ConfirmAndClose()
    {
        if (!IsEditing) return;

        MatchData.Instance.Deck = workingDeck;

        ClearViews();
        panel.SetActive(false);
        IsEditing = false;
    }

    private void ClearViews()
    {
        // clear card views from deck and rewards containers
        foreach (var v in deckViews) if (v != null) Destroy(v.gameObject);
        foreach (var v in rewardViews) if (v != null) Destroy(v.gameObject);
        deckViews.Clear();
        rewardViews.Clear();
    }

    private void UpdateUI()
    {
        changesText.text = $"Alterações: {changesMade}/{maxChanges}";
        addButton.interactable = HasChangesLeft && selectedReward != null;
        removeButton.interactable = HasChangesLeft && CanRemove && selectedDeckCard != null;
    }
}