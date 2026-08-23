using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickTimeEventSystem : Singleton<QuickTimeEventSystem>
{
    public List<Card> PlayedCards = new();

    [SerializeField] private SkillCheckUI skillCheckUI;
    [SerializeField] private GameObject Frame;
    [SerializeField] private float TimeLimit = 5f;

    [SerializeField] private List<KeyCode> keyInputs;
    private List<KeyCode> currentSequence = new List<KeyCode>();
    private KeyCode pressedKey = KeyCode.None;


    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
        ActionSystem.AttachPerformer<QuickTimeEventGA>(QuickTimeEventPerformer);
    }
    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
        ActionSystem.DetachPerformer<QuickTimeEventGA>();
    }
    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        QuickTimeEventGA quickTimeEventGA = new(PlayedCards);
        ActionSystem.Instance.AddReaction(quickTimeEventGA);
    }
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        // clear played cards list
        PlayedCards.Clear();
    }
    public IEnumerator QuickTimeEventPerformer(QuickTimeEventGA quickTimeEventGA)
    {
        // stop qte if you haven't played cards, or only played free cards
        // and also setup timer for qte
        if (quickTimeEventGA.Cards.Count == 0) { yield break; }
        int total = 0;
        int timeFlux = 0;
        foreach (var card in quickTimeEventGA.Cards)
        {
            total += card.Sequence;
            if (card.Sequence > 0) timeFlux++;
        }
        if (total == 0) { yield break; }

        TimeLimit = 99f + timeFlux;


        // clear any remaining sequence and set a new one
        currentSequence.Clear();
        foreach (var card in quickTimeEventGA.Cards)
        {
            List<KeyCode> cardKey = new List<KeyCode>();
            if (card.Sequence != 0)
            {
                for (int i = 0; i < card.Sequence; i++)
                {
                    KeyCode k = keyInputs[Random.Range(0, keyInputs.Count)];
                    cardKey.Add(k);
                    //Debug.Log($"Added {k.ToString()}");
                }

                currentSequence.AddRange(cardKey);
            }

            // space keycode = divisor
            currentSequence.Add(KeyCode.Space);
        }
        currentSequence.RemoveAt(currentSequence.Count - 1); // removes last divisor inserted

        // setup variables
        skillCheckUI.Setup(currentSequence);
        ShowQTEPrompt(true);
        float startTime = Time.time;
        float elapsedTime = 0f;


        Debug.Log($"Sequence count {currentSequence.Count}");

        for (int i = 0; i < currentSequence.Count; i++)
        {
            pressedKey = KeyCode.None;
            bool keyPressed = false;

            // divisor
            if (currentSequence[i] == KeyCode.Space)
            {
                OnKeyCorrect(i);
                continue;
            }

            // key press loop
            while (!keyPressed && elapsedTime < TimeLimit)
            {
                elapsedTime += Time.deltaTime;

                if (Input.anyKeyDown)
                {
                    // check if key is valid
                    foreach (KeyCode key in keyInputs)
                    {
                        if (Input.GetKeyDown(key))
                        {
                            pressedKey = key;
                        }
                    }
                    if (pressedKey == KeyCode.None) { continue; }
                    keyPressed = true;

                    if (Input.GetKeyDown(pressedKey) == Input.GetKeyDown(currentSequence[i]))
                    {
                        OnKeyCorrect(i);
                    }
                    else
                    {
                        OnKeyWrong(pressedKey);
                        yield break;
                    }
                }


                UpdateTimerUI(TimeLimit - elapsedTime);
                if (elapsedTime >= TimeLimit) OnKeyWrong();
                yield return null;
            }
        }
        // in case qte is complete, add reaction with bonuses
        OnQTESuccess();
        yield return null;
    }
    private void ShowQTEPrompt(bool active) => skillCheckUI.ShowQTEPrompt(active);
    private void UpdateTimerUI(float progress) => skillCheckUI.UpdateTimer(progress);
    private void OnKeyCorrect(int currentKeyIndex)
    {
        skillCheckUI.ChangeKeyColor(currentKeyIndex, Color.green);
        Debug.Log($"Correct! Pressed: {currentSequence[currentKeyIndex]}");


        // Play correct sound/effect
    }

    private void OnKeyWrong(KeyCode key = KeyCode.None)
    {
        if (key == KeyCode.None) Debug.Log("QTE Failed. Timed out");
        else Debug.Log($"QTE Failed. Pressed {key.ToString()}");

        skillCheckUI.ClearQTE();
        ShowQTEPrompt(false);
        // Play fail sound/effect
    }

    private void OnQTESuccess()
    {
        Debug.Log("QTE Complete Successfully!");

        skillCheckUI.ClearQTE();
        ShowQTEPrompt(false);
        // Play success sound/effect | Card will now be played
    }
}
