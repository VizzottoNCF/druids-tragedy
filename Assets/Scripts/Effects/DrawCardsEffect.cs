using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class DrawCardsEffect : Effect
{
    [SerializeField] private int drawAmount;
    public override GameAction GetGameAction(List<CombatantView> targets)
    {
        DrawCardGA drawCardGA = new(drawAmount);
        return drawCardGA;
    }
}
