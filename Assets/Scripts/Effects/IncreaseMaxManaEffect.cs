using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IncreaseMaxManaEffect : Effect
{
    [SerializeField] private int amount = 1;
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        return new IncreaseMaxManaGA(amount);
    }
}