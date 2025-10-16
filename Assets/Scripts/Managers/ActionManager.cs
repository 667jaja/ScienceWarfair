using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager instance;
    private List<GameAction> reactions = null;
    public bool isPerforming { get; private set; } = false;
    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();

    void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
    }
    public void Perform(GameAction action, System.Action OnPerformFinished = null)
    {
        if (isPerforming) return;
        isPerforming = true;
        StartCoroutine( Flow (action, () =>
        {
            isPerforming = false;
            OnPerformFinished?.Invoke();
        }));
    }
    public void AddReaction(GameAction gameAction)
    {
        reactions?.Add(gameAction);
    }

    //of extreme importance
    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        Debug.Log("flow Start");
        //sets "reactions" to the current actions prereactions
        reactions = action.preReactions;
        PerformSubscribers(action, preSubs);
        yield return PerformReactions();

        Debug.Log("flow activate");
        reactions = action.performReactions;
        yield return PerformPerformer(action);
        yield return PerformReactions();

        Debug.Log("flow close");
        reactions = action.postReactions;
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();

        Debug.Log("flow end");
        OnFlowFinished?.Invoke();
    }

    private IEnumerator PerformPerformer(GameAction action)
    {
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            yield return performers[type](action);
        }
    }
    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        //tells presubscribers that this action is in the PRE phases
        Type type = action.GetType();
        if (subs.ContainsKey(type))
        {
            Debug.Log("subs contains key");

            foreach (var sub in subs[type])
            {
                sub(action);
            }
        }
        else
        {
            Debug.Log("subs does not contain key");
        }
    }
    private IEnumerator PerformReactions()
    {
        //does all the actions in "reactions" with Flow
        Debug.Log("reactions count: " + reactions.Count);
        foreach (GameAction reaction in reactions)
        {
            yield return Flow(reaction);
        }
    }


    //called by other scripts to attach a performer so that it can be reacted to 
    //must subscribe OnEnable and unsubscribe OnDisable
    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        Debug.Log("Performer Attached");
        Type type = typeof(T);
        IEnumerator wrappedPerformer(GameAction action) => performer((T)action);
        if (performers.ContainsKey(type)) performers[type] = wrappedPerformer;
        else performers.Add(type, wrappedPerformer);
    }
    public static void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);
        if (performers.ContainsKey(type)) performers.Remove(type);
    }

    //called by other scripts to react to one of the performers
    //must subscribe OnEnable and unsubscribe OnDisable
    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Debug.Log("action Subscribed");
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        void wrappedReaction(GameAction action) => reaction((T)action);
        if (subs.ContainsKey(typeof(T)))
        {
            subs[typeof(T)].Add(wrappedReaction);
        }
        else
        {
            subs.Add(typeof(T), new());
            subs[typeof(T)].Add(wrappedReaction);
        }

    }
    public static void UnubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction(GameAction action) => reaction((T)action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
    }
}