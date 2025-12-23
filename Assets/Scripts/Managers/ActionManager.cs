using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    // [SerializeField] private int actionQueueCount;    
    // [SerializeField] private bool queueEnd1;    
    // [SerializeField] private bool isPerforming1;    
    public static ActionManager instance;
    private List<GameAction> reactions = null;
    private Queue<GameAction> actionQueue = new Queue<GameAction>();
    public bool isPerforming { get; private set; } = false;
    public bool queueEnd { get; private set; } = false;
    private bool currentActionIsQueueEnder = false;

    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();

    void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
    }
    // void Update()
    // {
    //     actionQueueCount = actionQueue.Count;
    //     queueEnd1 = queueEnd;
    //     isPerforming1 = isPerforming;
    // }
    public void Perform(GameAction action, System.Action OnPerformFinished = null)
    {
        //Debug.Log("EnQueueAction");

        if (queueEnd) return;
        if (action.isQueueEnder) queueEnd = true;

        actionQueue.Enqueue(action);

        if (!isPerforming)
        {
            PerformNextQueued();
        }
    }
    private void PerformNextQueued(System.Action OnPerformFinished = null)
    {
        //Debug.Log("DeQueueAction");
        GameAction action = actionQueue.Dequeue();
        isPerforming = true;

        StartCoroutine( Flow (action, () =>
        {
            isPerforming = false;
            if (currentActionIsQueueEnder) queueEnd = false;
            OnPerformFinished?.Invoke();
            
            if (actionQueue.Count > 0)
            {
                PerformNextQueued();
            }
        }));
    }
    public void AddReaction(GameAction gameAction)
    {
        reactions?.Add(gameAction);
    }

    //of extreme importance
    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        //Debug.Log("flow Start");
        //sets "reactions" to the current actions prereactions
        reactions = action.preReactions;
        //Debug.Log("flow PRE, reactions count: " + reactions.Count);
        PerformSubscribers(action, preSubs);
        yield return PerformReactions();

        reactions = action.performReactions;
        //Debug.Log("flow activate, reactions count: " + reactions.Count);
        yield return PerformPerformer(action);
        yield return PerformReactions();

        reactions = action.postReactions;
        //Debug.Log("flow POST, reactions count: " + reactions.Count);
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();

        if (action.isQueueEnder) currentActionIsQueueEnder = true;
        else currentActionIsQueueEnder = false;
        //Debug.Log("flow end, Action Queue Count: " + actionQueue.Count);
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
        //tells subscribers what phase this action is in
        Type type = action.GetType();
        if (subs.ContainsKey(type))
        {
            //Debug.Log("subs contains key");

            foreach (var sub in subs[type])
            {
                sub(action);
            }
        }
        else
        {
            //Debug.Log("subs does not contain key");
        }
    }
    private IEnumerator PerformReactions()
    {
        //does all the actions in "reactions" with Flow
        //Debug.Log("reactions count: " + reactions.Count);
        foreach (GameAction reaction in reactions)
        {
            yield return Flow(reaction);
        }
    }


    //called by other scripts to attach a performer so that it can be reacted to 
    //must subscribe OnEnable and unsubscribe OnDisable
    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        //Debug.Log("Performer Attached");
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
        //Debug.Log("action Subscribed");
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