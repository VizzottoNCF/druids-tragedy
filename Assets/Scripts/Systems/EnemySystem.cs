using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySystem : Singleton<EnemySystem>
{
    [SerializeField] private EnemyBoardView enemyBoardView;
    public List<EnemyView> Enemies => enemyBoardView.EnemyViews;
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<AttackHeroGA>(AttackHeroPerformer);
        ActionSystem.AttachPerformer<KillEnemyGA>(KillEnemyPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackHeroGA>();
        ActionSystem.DetachPerformer<KillEnemyGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    public void Setup(EnemyWave enemyData)
    {
        foreach (EnemyData enemy in enemyData.enemyDatas)
        {
            enemyBoardView.AddEnemy(enemy);
        }
    }


    private IEnumerator EnemyTurnPerformer(EnemyTurnGA action)
    {
        foreach (var enemy in enemyBoardView.EnemyViews)
        {
            // turn count for attacks
            enemy.TurnCount++;

            // apply burn
            int burnStacks = enemy.GetStatusEffectStackCount(StatusEffectType.BURN);
            if (burnStacks > 0)
            {
                ApplyBurnGA applyBurnGA = new(burnStacks, enemy);
                ActionSystem.Instance.AddReaction(applyBurnGA);

                if (enemy == null) continue;
            }

            // apply stun
            // always apply stun last of all effects
            int stunStacks = enemy.GetStatusEffectStackCount(StatusEffectType.STUN);
            if (stunStacks > 0)
            {
                ApplyStunGA applyStunGA = new(stunStacks, enemy);
                ActionSystem.Instance.AddReaction(applyStunGA);
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            

            // attack hero
            int attackCount = enemy.Behavior != null ? enemy.Behavior.GetAttackCount(enemy) : 1;
            for (int i = 0; i < attackCount; i++)
            {
                AttackHeroGA attackHeroGA = new(enemy);
                ActionSystem.Instance.AddReaction(attackHeroGA);
            }

            // special actions
            if (enemy.Behavior != null)
            {
                foreach (GameAction extra in enemy.Behavior.GetExtraActions(enemy))
                {
                    ActionSystem.Instance.AddReaction(extra);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
    private IEnumerator AttackHeroPerformer(AttackHeroGA attackHeroGA)
    {
        EnemyView attacker = attackHeroGA.Attacker;
        if (attacker == null) { yield break; }


        Vector3 targetPos = new(0,-2,0);
        Vector3 originalPos = attacker.transform.position;
        Tween tween = attacker.transform.DOMove(targetPos, 0.15f);
        yield return tween.WaitForCompletion();
        attacker.transform.DOMove(originalPos, 0.15f);

        DealDamageGA dealDamageGA = new(attacker.AttackPower, new() { HeroSystem.Instance.heroView }, attackHeroGA.Caster);
        ActionSystem.Instance.AddReaction(dealDamageGA);
    }
    private IEnumerator KillEnemyPerformer(KillEnemyGA killEnemyGA)
    {
        yield return enemyBoardView.RemoveEnemy(killEnemyGA.EnemyView);
    }

    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        if (enemyBoardView.EnemyViews.Count == 0)
        {
            Debug.Log("No enemies left");

            EndMatchGA endMatchGA = new(MatchSetupSystem.Instance.encounterID);
            ActionSystem.Instance.AddReaction(endMatchGA);
        }
        else // refill mana is here because it kept bugging out
        {
            // refill mana
            RefillManaGA refillManaGA = new();
            ActionSystem.Instance.AddReaction(refillManaGA);
        }
    }
}
