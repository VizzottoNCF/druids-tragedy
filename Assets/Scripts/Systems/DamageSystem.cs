using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSystem : Singleton<DamageSystem>
{
    [SerializeField] private GameObject damageVFX;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
    }
    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)
    {
        int amount = dealDamageGA.Amount;
        if (dealDamageGA.Caster != null)
            amount += dealDamageGA.Caster.GetStatusEffectStackCount(StatusEffectType.STRENGTH);

        foreach (var target in dealDamageGA.Targets)
        {
            if (target == null) continue;
            Vector3 pos = target.transform.position;
            target.Damage(amount);

            Instantiate(damageVFX, pos, Quaternion.identity);
            yield return new WaitForSeconds(0.15f);

            CheckKillTarget(target);
        }
    }

    public void CheckKillTarget(CombatantView target)
    {
        if (target.CurrentHealth <= 0)
        {
            if (target is EnemyView enemyView)
            {
                KillEnemyGA killEnemyGA = new(enemyView);
                ActionSystem.Instance.AddReaction(killEnemyGA);
            }
            else
            {
                Debug.Log("Game over logic");
            }
        }
    }
}
