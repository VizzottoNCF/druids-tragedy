using System;
using UnityEngine;

[Serializable]
public class BonusHitBehavior : EnemyBehavior
{
    [SerializeField] private int baseHits = 1;       // hits every turn
    [SerializeField] private int bonusEveryNTurns = 2;
    [SerializeField] private int bonusHits = 1;      // extra hits on those turns

    public override int GetAttackCount(EnemyView enemy)
    {
        bool isBonusTurn = enemy.TurnCount % bonusEveryNTurns == 0;
        return baseHits + (isBonusTurn ? bonusHits : 0);
    }
}