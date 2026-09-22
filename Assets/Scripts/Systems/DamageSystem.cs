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
        if (!dealDamageGA.QTESuccess) { yield break; }
        foreach (var target in dealDamageGA.Targets)
        {
            if (target == null) continue;
            Vector3 pos = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
            target.Damage(dealDamageGA.Amount);

            Instantiate(damageVFX, pos, Quaternion.identity);
            yield return new WaitForSeconds(0.15f);

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
}
