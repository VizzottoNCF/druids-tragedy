using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckEditSystem : Singleton<DeckEditSystem>
{
    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private DeckEditCardView cardViewPrefab;
    [SerializeField] private Transform rewardsContainer;
    [SerializeField] private Transform deckContainer;
    [SerializeField] private TMP_Text swapsText;

    [Header("Config")]
    [SerializeField] private int maxSwaps = 5;

    public bool IsEditing { get; private set; }

    private List<CardData> workingDeck;
    private readonly List<DeckEditCardView> deckViews = new();
    private readonly List<DeckEditCardView> rewardViews = new();
    private DeckEditCardView selectedReward, selectedDeckCard;
    private int swapsMade, swapsAllowed;

    protected override void Awake()
    {
        base.Awake();
        panel.SetActive(false);
    }

    public void StartEdit(List<CardData> rewards)
    {
        if (IsEditing) return;
        IsEditing = true;

        // trabalha numa cópia: o deck real só muda ao confirmar
        workingDeck = new List<CardData>(MatchSetupSystem.Instance.GetDeck());
        swapsMade = 0;
        swapsAllowed = Mathf.Min(maxSwaps, rewards.Count, workingDeck.Count);
        selectedReward = selectedDeckCard = null;

        ClearViews();
        for (int i = 0; i < workingDeck.Count; i++)
            deckViews.Add(SpawnView(workingDeck[i], i, deckContainer, OnDeckCardClicked));
        for (int i = 0; i < rewards.Count; i++)
            rewardViews.Add(SpawnView(rewards[i], i, rewardsContainer, OnRewardClicked));

        UpdateSwapsText();
        panel.SetActive(true);
    }

    private DeckEditCardView SpawnView(CardData data, int index, Transform parent, Action<DeckEditCardView> onClick)
    {
        DeckEditCardView view = Instantiate(cardViewPrefab, parent);
        view.Setup(data, index, onClick);
        return view;
    }

    private void OnRewardClicked(DeckEditCardView view)
    {
        if (swapsMade >= swapsAllowed) return;

        if (selectedReward == view)   // clicar de novo desmarca
        {
            view.SetSelected(false);
            selectedReward = null;
            return;
        }
        selectedReward?.SetSelected(false);
        selectedReward = view;
        view.SetSelected(true);
        TrySwap();
    }

    private void OnDeckCardClicked(DeckEditCardView view)
    {
        if (swapsMade >= swapsAllowed) return;

        if (selectedDeckCard == view)
        {
            view.SetSelected(false);
            selectedDeckCard = null;
            return;
        }
        selectedDeckCard?.SetSelected(false);
        selectedDeckCard = view;
        view.SetSelected(true);
        TrySwap();
    }

    private void TrySwap()
    {
        if (selectedReward == null || selectedDeckCard == null) return;

        CardData newCard = selectedReward.Data;
        int slot = selectedDeckCard.Index;   // índice, não CardData: o deck pode ter cartas repetidas
        workingDeck[slot] = newCard;

        // a carta do deck passa a mostrar a recompensa e fica bloqueada
        selectedDeckCard.Setup(newCard, slot, OnDeckCardClicked);
        selectedDeckCard.SetLocked(true);

        // a recompensa é consumida
        rewardViews.Remove(selectedReward);
        Destroy(selectedReward.gameObject);

        selectedReward = selectedDeckCard = null;
        swapsMade++;
        UpdateSwapsText();
    }

    // ligar no OnClick do botão "Concluir"
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
        foreach (var v in deckViews) if (v != null) Destroy(v.gameObject);
        foreach (var v in rewardViews) if (v != null) Destroy(v.gameObject);
        deckViews.Clear();
        rewardViews.Clear();
    }

    private void UpdateSwapsText() => swapsText.text = $"Trocas: {swapsMade}/{swapsAllowed}";
}