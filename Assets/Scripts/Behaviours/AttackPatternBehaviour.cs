using System;
using UnityEngine;

[Serializable]
public class AttackPatternBehavior : EnemyBehavior
{
    [SerializeField] private int hitsPerAttack = 1;
    [SerializeField] private int everyNTurns = 1;

    public override int GetAttackCount(EnemyView enemy)
    {
        return enemy.TurnCount % everyNTurns == 0 ? hitsPerAttack : 0;
    }
    public override int GetAttackCountForTurn(int turn) => turn % everyNTurns == 0 ? hitsPerAttack : 0;
}