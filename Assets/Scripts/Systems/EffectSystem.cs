using System.Collections;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    [SerializeField] private GameObject BurnVFX;
    [SerializeField] private GameObject StunVFX;
    private const int COUNTER_DAMAGE = 5;
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<PerformEffectGA>(PerformEffectPerformer);
        ActionSystem.AttachPerformer<ApplyBurnGA>(ApplyBurnPerformer);
        ActionSystem.AttachPerformer<ApplyStunGA>(ApplyStunPerformer);
        ActionSystem.SubscribeReaction<AttackHeroGA>(OnHeroAttackedPerformer, ReactionTiming.POST);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<PerformEffectGA>();
        ActionSystem.DetachPerformer<ApplyBurnGA>();
        ActionSystem.DetachPerformer<ApplyStunGA>();
        ActionSystem.UnsubscribeReaction<AttackHeroGA>(OnHeroAttackedPerformer, ReactionTiming.POST);
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
        if (target != null)
        {
            GameObject vfx = Instantiate(StunVFX, target.transform.position, Quaternion.identity);
            target.RemoveStatusEffect(StatusEffectType.STUN, 1);
            yield return new WaitForSeconds(1f);
            Destroy(vfx);
        }
    }
    private void OnHeroAttackedPerformer(AttackHeroGA attackGA)
    {
        HeroView hero = HeroSystem.Instance.heroView;
        if (hero.GetStatusEffectStackCount(StatusEffectType.COUNTER) <= 0) return;
        if (attackGA.Attacker == null) return;

        hero.RemoveStatusEffect(StatusEffectType.COUNTER, 1);

        DealDamageGA counter = new(COUNTER_DAMAGE, new() { attackGA.Attacker }, hero);
        ActionSystem.Instance.AddReaction(counter);
    }
}
