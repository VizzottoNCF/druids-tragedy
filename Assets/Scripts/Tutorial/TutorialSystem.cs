using UnityEngine;

public class TutorialSystem : Singleton<TutorialSystem>
{
    public static bool Active => Instance != null && Instance.running;
    [SerializeField] private TutorialUI ui;
    public TutorialData data;
    private int index;
    private bool running;
    private TutorialStep Current => data.Steps[index];
    public void Begin() 
    {
        Debug.Log("TutorialSystem.Begin");
        index = -1;
        running = true;
        Next();
    }
    private void Next()
    {
        if (index >= 0) Current.Completion?.Unsubscribe();
        index++;
        if (index >= data.Steps.Count) { running = false; ui.Hide(); return; }

        TutorialStep step = Current;
        if (step == null)
        {
            Debug.LogError($"Tutorial step {index} is null. Pick a type or remove the element.");
            return;
        }

        ui.Show(step.Message, step.WaitForClick, step.Target, Next);

        if (step.WaitForClick) return;

        if (step.Completion == null)
        {
            Debug.LogError($"Tutorial step {index} has no Completion and WaitForClick is off, so it can never advance.");
            return;
        }
        step.Completion.Subscribe(Next);
    }

    // queried by input code
    public bool CanUseCard(Card c) => !running || (Current.AllowedCard != null && c.Title == Current.AllowedCard.Title);
    public bool CanEndTurn() => !running || Current.AllowEndTurn;
}
