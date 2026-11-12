using UnityEngine;

public class StartTutorialMatchGA : GameAction
{
    public string EncounterID { get; private set; }
    public StartTutorialMatchGA(string id)
    {
        EncounterID = id;
    }
}
