using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class DealDamageEffect : Effect
{
    [SerializeField] private int DamageAmount;
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        DealDamageGA dealDamageGA = new(DamageAmount, targets, caster);
        return dealDamageGA;
    }
}
