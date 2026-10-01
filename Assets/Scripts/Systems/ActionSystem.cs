using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> reactions = null;
    public bool IsPerforming { get; private set; } = false;
    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();
    private static readonly Dictionary<(Type, ReactionTiming, Delegate), Action<GameAction>> wrappers = new();
    public void Perform(GameAction action, System.Action OnPerformFinished = null)
    {
        if (IsPerforming) return;
        IsPerforming = true;
        StartCoroutine(Flow(action, () => { IsPerforming = false; OnPerformFinished?.Invoke(); }));
    }
    public void AddReaction(GameAction gameAction) => reactions?.Add(gameAction);
    public void KillReactions() => reactions = null;
    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        reactions = action.PreReactions;
        PerformSubscribers(action, preSubs);
        yield return PerformReactions();

        reactions = action.PerformReactions;
        yield return PerformPerformer(action);
        yield return PerformReactions();

        reactions = action.PostReactions;
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();

        OnFlowFinished?.Invoke();
    }
    private IEnumerator PerformPerformer(GameAction action)
    {
        Type type = action.GetType();

        if (type != null && performers.ContainsKey(type))
        {
            yield return performers[type](action);
        }
        yield return null;
    }
    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type = action.GetType();
        if (subs.ContainsKey(type))
        {
            foreach (var sub in subs[type])
            {
                sub(action);
            }
        }
    }
    private IEnumerator PerformReactions()
    {
        if (reactions == null) { yield break; }

        // iterate through a copy of reactions because reactions can be killed by the EndMatch GameAction
        var reactionsCopy = reactions.ToList();

        foreach (var reaction in reactionsCopy)
        {
            if (reactions == null) { yield break; }

            yield return Flow(reaction);
        }
    }
    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        Type type = typeof(T);
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
        if (performers.ContainsKey(type)) { performers[type] = wrappedPerformer; }
        else { performers.Add(type, wrappedPerformer); }
    }
    public static void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);
        Debug.Log(type.Name);
        foreach (KeyValuePair<Type, Func<GameAction, IEnumerator>> p in performers)
        {
            if (p.Key.Name == type.Name)
            {
                print($"Removing {p.Key.Name}");
            }
        }
        if (performers.ContainsKey(type)) 
        { 
            
            performers.Remove(type);
            Debug.Log($"Performer removed {!performers.ContainsKey(type)}");
        }
    }
    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        var key = (typeof(T), timing, (Delegate)reaction);
        if (wrappers.ContainsKey(key)) return; // evita inscrição duplicada

        Action<GameAction> wrapped = action => reaction((T)action);
        wrappers[key] = wrapped;

        var subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (!subs.TryGetValue(typeof(T), out var list))
        {
            list = new List<Action<GameAction>>();
            subs[typeof(T)] = list;
        }
        list.Add(wrapped);
    }
    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        var key = (typeof(T), timing, (Delegate)reaction);
        if (!wrappers.TryGetValue(key, out var wrapped)) return;

        var subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.TryGetValue(typeof(T), out var list)) list.Remove(wrapped);
        wrappers.Remove(key);
    }
    /*public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        void wrappedReaction(GameAction action) => reaction((T)action);
        if (subs.ContainsKey(typeof(T))) { subs[typeof(T)].Add(wrappedReaction); }
        else
        {
            subs.Add(typeof(T), new());
            subs[typeof(T)].Add(wrappedReaction);
        }
    }
    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction(GameAction action) => reaction((T)action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
    }*/
}
