using UnityEngine;

public class EndMatchGA : GameAction
{
    public string EncounterID { get; private set; }
    public EndMatchGA(string id)
    {
        EncounterID = id;
    }
}
