using System.Collections.Generic;
using UnityEngine;

public class AddStatusEffectGA : GameAction, IHaveCaster
{
    public StatusEffectType StatusEffectType { get; private set; }
    public int StackCount { get; private set; }
    public List<CombatantView> Targets { get; private set; } = new List<CombatantView>();
    public CombatantView Caster { get; private set; }
    public AddStatusEffectGA(StatusEffectType statusEffectType, int stackCount, List<CombatantView> target, CombatantView caster)
    {
        StatusEffectType = statusEffectType;
        StackCount = stackCount;
        Targets = target;
        Caster = caster;
    }
}
