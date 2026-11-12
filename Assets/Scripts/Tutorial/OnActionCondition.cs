using System;
using UnityEngine;

[Serializable]
public abstract class OnActionCondition<T> : TutorialCondition where T : GameAction
{
    [SerializeField] private ReactionTiming timing = ReactionTiming.POST;

    private Action onMet;
    private Action<T> handler;

    public override void Subscribe(Action cb)
    {
        onMet = cb;
        handler = _ => onMet?.Invoke();
        ActionSystem.SubscribeReaction<T>(handler, timing);
    }

    public override void Unsubscribe()
    {
        if (handler == null) return;
        ActionSystem.UnsubscribeReaction<T>(handler, timing);
        handler = null;
        onMet = null;
    }
}