using System;
using System.Collections.Generic;
using TMPro;
using System.Linq;
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
    [SerializeField] private Button undoButton;
    [SerializeField] private GameObject CardViewUI;
    [SerializeField] private TMP_Text nomeGrande;
    [SerializeField] private TMP_Text descGrande;
    

    [Header("Config")]
    [SerializeField] private int maxChanges = 5;
    [SerializeField] private int minDeckSize = 8;

    public bool IsEditing { get; private set; }
    private class Snapshot
    {
        public List<CardData> Deck;
        public List<bool> Locked;
        public List<CardData> Rewards;
        public int ChangesMade;
    }

    private List<CardData> workingDeck;
    private readonly List<DeckEditCardView> deckViews = new();
    private readonly List<DeckEditCardView> rewardViews = new();
    private readonly Stack<Snapshot> history = new();
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

        history.Clear();

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

        SaveSnapshot();

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

        SaveSnapshot();

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
        
        SaveSnapshot();

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

        history.Clear();

        ClearViews();
        panel.SetActive(false);
        IsEditing = false;
    }

    // connect to OnClick of undo button in UI
    public void UndoLastChange()
    {
        if (!IsEditing || history.Count == 0) return;

        Snapshot s = history.Pop();
        workingDeck = new List<CardData>(s.Deck);
        changesMade = s.ChangesMade;
        selectedReward = selectedDeckCard = null;

        // rebuilds views based on snapshot data
        ClearViews();
        for (int i = 0; i < workingDeck.Count; i++)
        {
            DeckEditCardView v = SpawnView(workingDeck[i], i, deckContainer, OnDeckCardClicked);
            v.SetLocked(s.Locked[i]);
            deckViews.Add(v);
        }

        for (int i = 0; i < s.Rewards.Count; i++)
            rewardViews.Add(SpawnView(s.Rewards[i], i, rewardsContainer, OnRewardClicked));

        UpdateUI();
    }

    private void ClearViews()
    {
        // clear card views from deck and rewards containers
        foreach (var v in deckViews) if (v != null) Destroy(v.gameObject);
        foreach (var v in rewardViews) if (v != null) Destroy(v.gameObject);
        deckViews.Clear();
        rewardViews.Clear();
    }

    private void SaveSnapshot()
    {
        history.Push(new Snapshot
        {
            Deck = new List<CardData>(workingDeck),
            Locked = deckViews.Select(v => v.Locked).ToList(),
            Rewards = rewardViews.Select(v => v.Data).ToList(),
            ChangesMade = changesMade
        });
    }

    private void UpdateUI()
    {
        changesText.text = $"Trocas: {changesMade}/{maxChanges}";
        addButton.interactable = HasChangesLeft && selectedReward != null;
        removeButton.interactable = HasChangesLeft && CanRemove && selectedDeckCard != null;
        undoButton.interactable = history.Count > 0;
    }

    public void ShowCardUI(CardData data)
    {
        Card c = new Card(data);
        CardViewUI.GetComponent<CardView>().Setup(c);
        nomeGrande.text = data.Title;
        descGrande.text = data.Description;
    }
}