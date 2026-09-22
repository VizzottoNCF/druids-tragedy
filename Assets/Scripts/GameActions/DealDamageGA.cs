using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageGA : GameAction, IHaveCaster
{
    public int Amount;
    public bool QTESuccess;
    public List<CombatantView> Targets { get; set; }
    public CombatantView Caster { get; private set; }
    public DealDamageGA(int amount, List<CombatantView> targets, CombatantView caster, bool qte = true)
    { 
        Amount = amount;
        Targets = targets;
        Caster = caster;
        QTESuccess = qte;
    }
}
