using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public CardData cardData { get; private set;}
    public int cardInstanceId;

    //constructor
    public Card(CardData cardData)
    {
        this.cardData = cardData;
        cardInstanceId = Random.Range(0, 2000000000);
        //cardEffect = cardData.cardEffect;
        PlacementCost = cardData.placementCost;
        Iq = cardData.iq;
        Health = cardData.health;
        //noAttack = cardData.noAttack;
        effectTriggers = new List<EffectTrigger>();
        containedCard = cardData.containedCard;
    }

    //non gameplay elements
    public Sprite cardArt { get => cardData.cardArt; }
    public string title { get => cardData.title; }
    public string description { get => cardData.description; }

    //Action vs Unit
    public bool isAction { get => cardData.isAction; }

    //stats
    public int placementCost { get; set; }
    public int iq { get; set; }
    public int health { get; set; }
    public int PlacementCost
    {
        get
        {
            return placementCost;
        }
        set
        {
            if (value < 0)
            {
                placementCost = 0;
            }
            else
            {
                placementCost = value;
            }
        }
    }
    public int Iq
    {
        get
        {
            return iq;
        }
        set
        {
            if (value < 0)
            {
                iq = 0;
            }
            else
            {
                iq = value;
            }
        }
    }
    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            if (value < 0)
            {
                health = 0;
            }
            else
            {
                health = value;
            }
        }
    }

    //abilities
    public bool noAttack { get => cardData.noAttack; }
    public string cardEffect { get => cardData.cardEffect; }
    public List<EffectTrigger> effectTriggers {get; set;}
    public CardData containedCard { get; set; }

    //deck Creator
    public bool isInADeck { get; set; }

    public void PlacementAbility(ActionData actionData)// Card card
    {
        actionData.originCard = this;
        Debug.Log("placementCalled");

            foreach (EffectTriggerData effectTriggerData in cardData.effectTriggerDatas)
            {
                if (effectTriggerData != null)
                {
                    EffectTrigger newEffectTrigger = new EffectTrigger(actionData, effectTriggerData);
                    newEffectTrigger.Placement(actionData);
                    effectTriggers.Add(newEffectTrigger);
                }
            }

    }
    public void Destruciton()
    {
        foreach (EffectTrigger effectTrigger in effectTriggers)
        {
            if (effectTrigger != null)
            {
                effectTrigger.DestructionEffect();
            }
        }
    }
    public void UnsubET()
    {
        foreach (EffectTrigger effectTrigger in effectTriggers)
        {
            if (effectTrigger != null)
            {
                effectTrigger.Unsub();
            }
        }
    }
}

    // public void PerformAbility(ActionData actionData)
    // {
    //     // Debug.Log("card " + cardInstanceId.ToString() + " performs " + effects.Count +" effects");
    //     // bool actionDescribed = false;
    //     // foreach (Effect effect in effects)
    //     // {
            
    //     //     if (effect != null)
    //     //     {
                
    //     //         effect.actionData = actionData;
    //     //         foreach (GameAction action in effect.effect)
    //     //         {
    //     //             action.inputPlayerId = actionData.originPlayerId;
    //     //             if (!actionDescribed)
    //     //             {
    //     //                 action.description = title + " Performs: " + cardEffect;
    //     //                 actionDescribed = true;
    //     //             }
    //     //             ActionManager.instance.Perform(action);
    //     //         }
    //     //     }
    //     //     else
    //     //     {
    //     //         Debug.Log("null effects");
    //     //     }
    //     // }
    // }