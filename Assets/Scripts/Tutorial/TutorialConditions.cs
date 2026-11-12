using System;

[Serializable] public class OnSpendManaCondition : OnActionCondition<SpendManaGA> { }
[Serializable] public class OnEnemyTurnCondition : OnActionCondition<EnemyTurnGA> { }
[Serializable] public class OnQuickTimeEventCondition : OnActionCondition<QuickTimeEventGA> { }
[Serializable] public class OnAddStatusEffectCondition : OnActionCondition<AddStatusEffectGA> { }
[Serializable] public class OnEndMatchCondition : OnActionCondition<EndMatchGA> { }
[Serializable] public class OnPlayCardCondition : OnActionCondition<PlayCardGA> { }
[Serializable] public class OnDealDamageCondition : OnActionCondition<DealDamageGA> { }