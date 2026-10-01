using UnityEngine;

public class StartMatchGA : GameAction
{
    public string EncounterID {  get; private set; }
    public StartMatchGA(string id)
    {
        EncounterID = id;
    }
}
