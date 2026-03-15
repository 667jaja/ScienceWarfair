using System.Collections.Generic;
using UnityEngine;

public class EffectTrigger
{
    public EffectTriggerData effectTriggerData;
    public List<Effect> effects { get; set;}

    public bool pre { get => effectTriggerData.pre; }
    public EffectTriggerType effectTriggerType { get => effectTriggerData.effectTriggerType; }
    private bool oneTimeUse { get => effectTriggerData.oneTimeUse; }
    public int countDown { get => effectTriggerData.countDown; }
    public int countDownVal { get; set;}

    private bool targetEnemyOnly {get => effectTriggerData.targetEnemyOnly;}
    private bool targetMeOnly {get => effectTriggerData.targetMeOnly;}

    public int originUnitInstanceId{ get; set;}
    public Card originCard{ get; set;}
    public int originPlayer;

    public bool triggerDisabled;
    public bool subbed = false;
    public string effectDescription { get => effectTriggerData.effectDescription; }

    public EffectTrigger(ActionData actionData, EffectTriggerData newEffectTriggerData)
    {
        effectTriggerData = newEffectTriggerData;
        countDownVal = newEffectTriggerData.countDownVal;
        effects = newEffectTriggerData.effects;

        originPlayer = actionData.originPlayerId;
        if (actionData.originCard != null)originUnitInstanceId = actionData.originCard.cardInstanceId;
        originCard = actionData.originCard;

        Sub();
    }
    public void TriggerEffect(ActionData actionData)
    {
        // if (originCard == null) originCard = UnitManager.instance.GetUnitByInstanceId(originUnitInstanceId);
        // originCard.PerformAbility(actionData);

        Debug.Log("card " + originUnitInstanceId.ToString() + " performs " + effects.Count +" effects");
        bool actionDescribed = false;
        foreach (Effect effect in effects)
        {
            
            if (effect != null)
            {
                effect.actionData = actionData;
                foreach (GameAction action in effect.effect)
                {
                    action.inputPlayerId = actionData.originPlayerId;
                    if (!actionDescribed)
                    {
                        action.description = originCard.title + " Performs: " + effectDescription;
                        actionDescribed = true;
                    }
                    ActionManager.instance.Perform(action);
                }
            }

        }
    }
    public void Placement(ActionData actionData)
    {
        PlacementEffect(actionData);
    }
    public void PlacementEffect(ActionData actionData)
    { 
        if (effectTriggerType == EffectTriggerType.Placement)
        {
            TriggerEffect(actionData);
            triggerDisabled = true;
            subbed = false;
        }
    }
    public void DestructionEffect()
    {
        Unsub();
    }
    public void Sub()
    {
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
        if (originCard == null) originCard = UnitManager.instance.GetUnitByInstanceId(originUnitInstanceId);
        ActionData actionData = new ActionData(originPlayer, new Vector2Int(0,0), originCard);
        actionData.targetPlayerId = startTurnGA.playerId;
        Reaction(actionData);
    }
    private void CardPlacedReaction(CreateUnitGA createUnitGA)
    {
        if (originCard == null) originCard = UnitManager.instance.GetUnitByInstanceId(originUnitInstanceId);
        ActionData actionData = new ActionData(originPlayer, new Vector2Int(0,0), originCard)
        {
            targetPlayerId = createUnitGA.playerId,
            targetCard = createUnitGA.playedCard,
            targetPosition = createUnitGA.position,
        };

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
                if (GameManager.instance.players[originPlayer].units[i,j] != null && GameManager.instance.players[originPlayer].units[i,j].cardInstanceId == originUnitInstanceId)
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
        if (originPositionFound) UnitManager.instance.UpdateCardVisual(actionData.originPlayerId, actionData.originPosition);
        else UnitManager.instance.UpdateAllCardVisuals();
    }

    public string GetTriggerName()
    {
        string before = "";
        string oneTime = "";
        string team = "";
        string type = "";
        //Before Next Friendly Unit Created
        if (pre) before = "Before "; 
        if (oneTimeUse) oneTime = "Next "; 
        if (targetMeOnly) team = "Friendly ";
        if (targetEnemyOnly) team = "Enemy ";
        if (effectTriggerType == EffectTriggerType.StartTurn) type = "Turn Start"; 
        if (effectTriggerType == EffectTriggerType.CardPlaced) type = "Unit Created"; 

        if (effectTriggerType == EffectTriggerType.Placement)
        {
            before = "";
            oneTime = "";
            team = "";
            type = "This Played"; 
        } 
        if (targetMeOnly && targetEnemyOnly)
        {
            before = "";
            oneTime = "";
            team = "Never";
            type = ""; 
        } 

        return before + oneTime + team + type;
        
    }
    public string GetEffectName()
    {
        return effectDescription;
    }
}