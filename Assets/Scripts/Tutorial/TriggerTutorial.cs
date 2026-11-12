using UnityEngine;

public class TriggerTutorial : MonoBehaviour
{
    private void Start()
    {
        StartTutorialMatchGA startTutorialMatchGA = new StartTutorialMatchGA("1");
        ActionSystem.Instance.Perform(startTutorialMatchGA);
    }
}
