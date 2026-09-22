using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChangeDefenseEffect : Effect
{
    [SerializeField] private int NewDefense;
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        ChangeDefenseGA changeDefenseGA = new(NewDefense, targets, caster);
        return changeDefenseGA;
    }
}