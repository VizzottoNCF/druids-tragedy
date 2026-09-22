using System.Collections.Generic;
using UnityEngine;

public class ChangeDefenseGA : GameAction, IHaveCaster
{
    public int Amount;
    public List<CombatantView> Targets { get; set; }
    public CombatantView Caster { get; private set; }
    public ChangeDefenseGA(int amount, List<CombatantView> targets, CombatantView caster)
    {
        Amount = amount;
        Targets = targets;
        Caster = caster;
    }
}
