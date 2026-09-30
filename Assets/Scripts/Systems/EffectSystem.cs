using System.Collections;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    [SerializeField] private GameObject BurnVFX;
    [SerializeField] private GameObject StunVFX;
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<PerformEffectGA>(PerformEffectPerformer);
        ActionSystem.AttachPerformer<ApplyBurnGA>(ApplyBurnPerformer);
        ActionSystem.AttachPerformer<ApplyStunGA>(ApplyStunPerformer);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<PerformEffectGA>();
        ActionSystem.DetachPerformer<ApplyBurnGA>();
        ActionSystem.DetachPerformer<ApplyStunGA>();
    }
    private IEnumerator PerformEffectPerformer(PerformEffectGA performEffectGA)
    {
        GameAction effectAction = performEffectGA.Effect.GetGameAction(performEffectGA.Targets, HeroSystem.Instance.heroView);
        ActionSystem.Instance.AddReaction(effectAction);
        yield return null;
    }
    private IEnumerator ApplyBurnPerformer(ApplyBurnGA applyBurnGA)
    {
        CombatantView target = applyBurnGA.Target;
        GameObject vfx = Instantiate(BurnVFX, target.transform.position, Quaternion.identity);
        target.Damage(applyBurnGA.BurnDamage);
        target.RemoveStatusEffect(StatusEffectType.BURN, 1);
        yield return new WaitForSeconds(1f);
        Destroy(vfx);

        // check if enemy died mid turn
        DamageSystem.Instance.CheckKillTarget(target);
    }
    private IEnumerator ApplyStunPerformer(ApplyStunGA applyStunGA)
    {
        CombatantView target = applyStunGA.Target;
        GameObject vfx = Instantiate(StunVFX, target.transform.position, Quaternion.identity);
        target.RemoveStatusEffect(StatusEffectType.STUN, 1);
        yield return new WaitForSeconds(1f);
        Destroy(vfx);
    }
}
