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

    private bool targetEnemyOnly {get => effectTriggerData.targetEnemyOnly;}
    private bool targetMeOnly {get => effectTriggerData.targetMeOnly;}

    public Card originCard{ get; private set;}
    public int originPlayer;

    private bool triggerDisabled;
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
            subbed = true;
            if (pre) ActionManager.SubscribeReaction<StartTurnGA>(StartTurnReaction, ReactionTiming.PRE);
            ActionManager.SubscribeReaction<StartTurnGA>(StartTurnReaction, ReactionTiming.POST);
        }
        if (effectTriggerType == EffectTriggerType.CardPlaced)
        {
            Debug.Log("subbed card effect to CardPlacedGA");
            subbed = true;
            if (pre) ActionManager.SubscribeReaction<CreateUnitGA>(CardPlacedReaction, ReactionTiming.PRE);
            ActionManager.SubscribeReaction<CreateUnitGA>(CardPlacedReaction, ReactionTiming.POST);
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
                if (pre) ActionManager.UnsubscribeReaction<StartTurnGA>(StartTurnReaction, ReactionTiming.PRE);
                ActionManager.UnsubscribeReaction<StartTurnGA>(StartTurnReaction, ReactionTiming.POST);
            }
            if (effectTriggerType == EffectTriggerType.CardPlaced)
            {
                Debug.Log("unsubbed card effect from CreateUnitGA");
                if (pre) ActionManager.UnsubscribeReaction<CreateUnitGA>(CardPlacedReaction, ReactionTiming.PRE);
                ActionManager.UnsubscribeReaction<CreateUnitGA>(CardPlacedReaction, ReactionTiming.POST);
            }
        }
    }

    private void StartTurnReaction(StartTurnGA startTurnGA)
    {
        ActionData actionData = new ActionData(originPlayer, new Vector2Int(0,0), originCard);
        actionData.targetPlayerId = startTurnGA.playerId;
        Reaction(actionData);
    }
    private void CardPlacedReaction(CreateUnitGA createUnitGA)
    {
        ActionData actionData = new ActionData(originPlayer, new Vector2Int(0,0), originCard);
        actionData.targetPlayerId = createUnitGA.playerId;
        actionData.targetCard = createUnitGA.playedCard;
        actionData.targetPositions = new List<Vector2Int> {createUnitGA.position};
        Reaction(actionData);
    }
    private void Reaction(ActionData actionData)
    {
        //find originCard location (if none found assume 0,0)
        bool originPositionFound = false;
        for (int i = 0; i < UnitManager.instance.columnCount; i++)
        {
            //for every column
            for (int j = 0; j < UnitManager.instance.rowCount; j++)
            {
                //for every row
                if (GameManager.instance.players[originPlayer].units[i,j] == originCard)
                {
                    actionData.originPosition = new Vector2Int(i,j);
                    originPositionFound = true;
                    break;
                }
            }
            if (originPositionFound) break;
        }
        if (!originPositionFound) Debug.Log("No Origin Position Found");

        if ((actionData.targetPlayerId == originPlayer || !targetMeOnly) && (actionData.targetPlayerId != originPlayer || !targetEnemyOnly) && !triggerDisabled)
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