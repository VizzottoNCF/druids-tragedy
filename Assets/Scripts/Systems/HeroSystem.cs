using UnityEngine;

public class HeroSystem : Singleton<HeroSystem>
{
    [field: SerializeField] public HeroView heroView { get; private set; }
    public void Setup(HeroData heroData)
    {
        heroView.Setup(heroData);
    }
    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }
    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }
    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        // discard all cards and trigger QTEs
        DiscardAllCardsGA discardAllCardsGA = new();
        ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        // apply strength
        //int strengthStacks = heroView.GetStatusEffectStackCount(StatusEffectType.STRENGTH);
        //if (strengthStacks > 0)
        //{
        //    heroView.RemoveStatusEffect(StatusEffectType.STRENGTH, 1);
        //}

        // apply burn
        int burnStacks = heroView.GetStatusEffectStackCount(StatusEffectType.BURN);
        if (burnStacks > 0)
        {
            ApplyBurnGA applyBurnGA = new(burnStacks, heroView);
            ActionSystem.Instance.AddReaction(applyBurnGA);
        }

        // apply stun
        int stunStacks = heroView.GetStatusEffectStackCount(StatusEffectType.STUN);
        if (stunStacks > 0)
        {
            ApplyStunGA applyStunGA = new(stunStacks, heroView);
            ActionSystem.Instance.AddReaction(applyStunGA);


            ActionSystem.Instance.Perform(new EnemyTurnGA());
        }
        else // if not stunned, draw cards
        {
            // draw cards
            DrawCardGA drawCardGA = new(5);
            ActionSystem.Instance.AddReaction(drawCardGA);
        }
    }
}