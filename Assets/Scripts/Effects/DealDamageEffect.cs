using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class DealDamageEffect : Effect
{
    [SerializeField] private int DamageAmount;
    public override GameAction GetGameAction(List<CombatantView> targets)
    {
        DealDamageGA dealDamageGA = new(DamageAmount, targets);
        return dealDamageGA;
    }
}
