using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackHeroGA>();
        ActionSystem.DetachPerformer<KillEnemyGA>();
    }

    public void Setup(List<EnemyData> enemyData)
    {
        foreach (EnemyData enemy in enemyData)
        {
            enemyBoardView.AddEnemy(enemy);
        }
    }


    private IEnumerator EnemyTurnPerformer(EnemyTurnGA action)
    {
        Debug.Log("Enemy turn start.");
        foreach (var enemy in enemyBoardView.EnemyViews)
        {
            Debug.Log($"{enemy.gameObject.name}'s turn");
            AttackHeroGA attackHeroGA = new(enemy);
            ActionSystem.Instance.AddReaction(attackHeroGA);
            yield return new WaitForSeconds(0.1f);
        }

                
        Debug.Log("Enemy turn finished.");
        yield return null;
    }
    private IEnumerator AttackHeroPerformer(AttackHeroGA attackHeroGA)
    {
        EnemyView attacker = attackHeroGA.Attacker;
        Vector3 targetPos = new(0,0,0);
        Vector3 originalPos = attacker.transform.position;
        Tween tween = attacker.transform.DOMove(targetPos, 0.15f);
        yield return tween.WaitForCompletion();
        attacker.transform.DOMove(originalPos, 0.15f);

        DealDamageGA dealDamageGA = new(attacker.AttackPower, new() { HeroSystem.Instance.heroView });
        ActionSystem.Instance.AddReaction(dealDamageGA);
    }
    private IEnumerator KillEnemyPerformer(KillEnemyGA killEnemyGA)
    {
        yield return enemyBoardView.RemoveEnemy(killEnemyGA.EnemyView);
    }
}
