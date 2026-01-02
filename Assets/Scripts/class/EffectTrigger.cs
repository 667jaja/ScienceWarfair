using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "EffectTrigger", menuName = "Effect Trigger")]
public enum EffectTriggerType
{
    Simple = 0,
    StartTurn = 1
}
[System.Serializable]
public class EffectTrigger
{
    [field: SerializeField] public bool pre {get; set;}
    [field: SerializeField] public List<Card> subscribedCards{ get; private set;}
    [field: SerializeField] public EffectTriggerType effectTriggerType;
    [field: HideInInspector] public int originPlayer;
    private bool isInPlay = false;

    [field: SerializeField] private bool oneTimeUse {get; set;} = false;
    [field: HideInInspector] private bool onlyMyTurn {get; set;} = true;
    [field: SerializeField] private int countDown {get; set;} = 0;
    [field: SerializeField] private int countDownVal = 0;
    private bool triggerDisabled;

    // public EffectTrigger(Card originCard)
    // {
    //     subscribedCards = new List<Card>();
    //     subscribedCards.Add(originCard);
    //     pre = false;
    // }
    private void NotifySubscribers(ActionData actionData)
    {
        foreach (Card card in subscribedCards)
        {
            card.PerformAbility(actionData);
        }
    }
    public void TriggerEffect(ActionData actionData)
    {
        if (isInPlay)
        {
            NotifySubscribers(actionData);
        }
    }
    public void Placement(ActionData actionData)
    {
        isInPlay = true;
        subscribedCards = new List<Card>();
        subscribedCards.Add(actionData.originCard);
        pre = false;
        PlacementEffect(actionData);
    }
    public virtual void PlacementEffect(ActionData actionData)
    {
        if (effectTriggerType == EffectTriggerType.Simple)
        {
            TriggerEffect(actionData);
        }
        if (effectTriggerType == EffectTriggerType.StartTurn)
        {
            onlyMyTurn = true;
            if (pre) ActionManager.SubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.PRE);
            else ActionManager.SubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.POST);
        }
    }
    public virtual void DestructionEffect()
    {
        if (effectTriggerType == EffectTriggerType.StartTurn)
        {
            if (pre) ActionManager.UnubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.PRE);
            else ActionManager.UnubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.POST);
        }
    }

    private void Reaction(StartTurnGA startTurnGA)
    {
        ActionData actionData = new ActionData(originPlayer, new Vector2Int(0,0), null);
        if ((startTurnGA.playerId == originPlayer || !onlyMyTurn) && !triggerDisabled)
        {
            if (countDownVal <= 0)
            {
                TriggerEffect(actionData);
                if (oneTimeUse) triggerDisabled = true;
                countDownVal = countDown;
                
            }
            else
            {
                countDown -= 1;
            }
        }
    }
}
    // private void PerformEffect(ActionData actionData)
    // {
    //     if (triggeredEffects.Count > 0)
    //     {
    //         foreach (Effect effect in triggeredEffects)
    //         {
    //             if (effect != null)
    //             {
    //                 effect.actionData = actionData;
    //                 foreach (GameAction action in effect.effect)
    //                 {
    //                     ActionManager.instance.Perform(action);
    //                 }
    //             }
    //         }
    //     } 
    // }