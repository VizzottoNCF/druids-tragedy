using System;
using System.Collections.Generic;

[Serializable]
public abstract class EnemyBehavior
{
    // How many times the enemy attacks the hero this turn. 0 = skips the attack.
    public virtual int GetAttackCount(EnemyView enemy) => 1;

    // Special actions to run alongside the attack (armor, status effects, etc.)
    public virtual List<GameAction> GetExtraActions(EnemyView enemy) => new();
}