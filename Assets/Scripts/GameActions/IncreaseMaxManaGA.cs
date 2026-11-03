using UnityEngine;

public class IncreaseMaxManaGA : GameAction
{
    public int Amount { get; private set; }
    public IncreaseMaxManaGA(int amount) { Amount = amount; }
}
