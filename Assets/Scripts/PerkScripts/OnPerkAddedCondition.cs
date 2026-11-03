using System;

[Serializable]
public class OnPerkAddedCondition : PerkCondition
{
    public override bool SubConditionIsMet() => true;
    public override void SubscribeCondition(Action<GameAction> reaction) => reaction?.Invoke(null);
    public override void UnsubscribeCondition(Action<GameAction> reaction) { }
}