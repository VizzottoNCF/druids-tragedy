using UnityEngine;

public class IncreaseMaxTrayGA : GameAction
{
    public int Amount { get; private set; }
    public IncreaseMaxTrayGA(int amount) { Amount = amount; }
}
