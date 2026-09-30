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

    public bool succededQTE = true;


    private void OnEnable()
    {
        ActionSystem.AttachPerformer<QuickTimeEventGA>(QuickTimeEventPerformer);
        ActionSystem.SubscribeReaction<PlayCardGA>(PlayCardPreReaction, ReactionTiming.PRE);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<QuickTimeEventGA>();
        ActionSystem.UnsubscribeReaction<PlayCardGA>(PlayCardPreReaction, ReactionTiming.PRE);
    }
    private void PlayCardPreReaction(PlayCardGA playCardGA)
    {
        // clear QTEs, and add new qte to system
        PlayedCards.Clear();
        PlayedCards.Add(playCardGA.Card);

        // play qte
        QuickTimeEventGA quickTimeEventGA = new(PlayedCards);
        ActionSystem.Instance.AddReaction(quickTimeEventGA);
    }
    public IEnumerator QuickTimeEventPerformer(QuickTimeEventGA quickTimeEventGA)
    {
        // if there no cards played, break
        if (quickTimeEventGA.Cards.Count == 0) { yield break; }
        
        // timer setup
        int timeFlux = 0;
        foreach (var card in quickTimeEventGA.Cards)
        {
            timeFlux++;
        }

        TimeLimit = 5f + timeFlux;


        // clear any remaining sequence and add the keys to the qte sequence
        currentSequence.Clear();
        foreach (var card in quickTimeEventGA.Cards)
        {

            // add Keys to currentSequence
            card.KeySequence.ForEach(k => currentSequence.Add(k));
        }


        // setup variables
        skillCheckUI.Setup(currentSequence);
        ShowQTEPrompt(true);
        float startTime = Time.time;
        float elapsedTime = 0f;


        for (int i = 0; i < (currentSequence.Count); i++)
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

        // Play correct sound/effect
    }

    private void OnKeyWrong(KeyCode key = KeyCode.None)
    {
        succededQTE = false;

        skillCheckUI.ClearQTE();
        ShowQTEPrompt(false);
    }

    private void OnQTESuccess()
    {
        succededQTE = true;

        skillCheckUI.ClearQTE();
        ShowQTEPrompt(false);
    }
}
