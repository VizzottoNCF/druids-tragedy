using System.Collections.Generic;

public class QuickTimeEventGA : GameAction
{
    public List<Card> Cards { get; private set; }
    public QuickTimeEventGA(List<Card> cards)
    {
        Cards = cards;
    }
}
