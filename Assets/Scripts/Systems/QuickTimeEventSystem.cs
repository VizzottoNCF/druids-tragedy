using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private float timeSinceLastWrongKeyPress = 0f;
    private float timeTolerance = 0.35f;

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

        // clear any remaining sequence and add the keys to the qte sequence
        currentSequence.Clear();


        // add Keys to currentSequence and setup timer
        TimeLimit = 1f;
        foreach (var card in quickTimeEventGA.Cards)
        {
            card.KeySequence.ForEach(k =>
            {
                currentSequence.Add(k);
                TimeLimit += 1f;
            });
        }

        // delay consecutive QTEs to avoid accidental key presses
        if (Time.time - timeSinceLastWrongKeyPress < timeTolerance)
        {
            yield return new WaitForSeconds(timeTolerance);
        }

        // setup variables
        skillCheckUI.Setup(currentSequence, quickTimeEventGA.Cards[0]);
        ShowQTEPrompt(true);
        float startTime = Time.time;
        float elapsedTime = 0f;


        // automatically end QTE in case all enemy views are already dead
        if (EnemySystem.Instance.Enemies.Count == 0)
        {
            OnQTESuccess();
            yield break;
        }



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
                if (TutorialSystem.Instance == null) elapsedTime += Time.deltaTime;

                if (Input.anyKeyDown)
                {
                    // check if key is valid
                    pressedKey = KeyCode.None;
                    foreach (KeyCode key in keyInputs)
                    {
                        if (Input.GetKeyDown(key)) { pressedKey = key; break; }
                    }


                    // ignore non valid  keys
                    if (pressedKey != KeyCode.None)
                    {
                        bool correct = pressedKey == currentSequence[i];


                        //tutorial: wrong keys are ignored
                        if (TutorialSystem.Instance != null)
                        {
                            if (correct) { OnKeyCorrect(i); keyPressed = true; }
                        }

                        // default
                        else
                        {
                            keyPressed = true;
                            if (correct) OnKeyCorrect(i);
                            else { OnKeyWrong(pressedKey); yield break; }
                        }
                    }
                }


                UpdateTimerUI(TimeLimit - elapsedTime);
                if (elapsedTime >= TimeLimit) OnKeyWrong();
                yield return null;
            }
        }

        // if time did not run out, then the player succeeded the QTE
        if (elapsedTime < TimeLimit) OnQTESuccess();
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
        if (TutorialSystem.Instance != null) return;

        timeSinceLastWrongKeyPress = Time.time;

        succededQTE = false;

        skillCheckUI.ClearQTE();
        skillCheckUI.FailQTEVisualFX();
        ShowQTEPrompt(false);

    }

    private void OnQTESuccess()
    {
        succededQTE = true;

        skillCheckUI.SuccessQTEVisualFX();
        skillCheckUI.ClearQTE();
        ShowQTEPrompt(false);
    }
}
