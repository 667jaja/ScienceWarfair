using System.Collections.Generic;
using UnityEngine;

public class EffectTrigger
{
    public EffectTriggerData effectTriggerData;

    public bool pre { get => effectTriggerData.pre; }
    public EffectTriggerType effectTriggerType { get => effectTriggerData.effectTriggerType; }
    private bool oneTimeUse { get => effectTriggerData.oneTimeUse; }
    private int countDown { get => effectTriggerData.countDown; }
    private int countDownVal { get; set;}

    public Card originCard{ get; private set;}
    public int originPlayer;

    private bool triggerDisabled;
    private bool onlyMyTurn;
    private bool isInPlay = false;
    private bool subbed;

    public EffectTrigger(ActionData actionData, EffectTriggerData newEffectTriggerData)
    {
        effectTriggerData = newEffectTriggerData;
        countDownVal = newEffectTriggerData.countDownVal;

        originPlayer = actionData.originPlayerId;
        originCard = actionData.originCard;
        Placement(actionData);
    }
    public void TriggerEffect(ActionData actionData)
    {
        if (isInPlay)
        {
            originCard.PerformAbility(actionData);
        }
    }
    public void Placement(ActionData actionData)
    {
        isInPlay = true;
        PlacementEffect(actionData);
    }
    public void PlacementEffect(ActionData actionData)
    {
        if (effectTriggerType == EffectTriggerType.Simple)
        {
            TriggerEffect(actionData);
            subbed = false;
        }
        if (effectTriggerType == EffectTriggerType.StartTurn)
        {
            Debug.Log("subbed card effect to StartTurnGA");
            onlyMyTurn = true;
            subbed = true;
            if (pre) ActionManager.SubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.PRE);
            ActionManager.SubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.POST);
        }
    }
    public void DestructionEffect()
    {
        Unsub();
    }
    public void Unsub()
    {
        if (subbed)
        {
            subbed = false;
            if (effectTriggerType == EffectTriggerType.StartTurn)
            {
                Debug.Log("unsubbed card effect from StartTurnGA");
                if (pre) ActionManager.UnsubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.PRE);
                ActionManager.UnsubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.POST);
            }
        }
    }

    private void Reaction(StartTurnGA startTurnGA)
    {
        ActionData actionData = new ActionData(originPlayer, new Vector2Int(0,0), originCard);
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
                countDownVal -= 1;
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