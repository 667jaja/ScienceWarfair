using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public CardData cardData { get; private set;}
    public int cardInstanceId;

    //constructor
    public Card(CardData cardData)
    {
        //basic
        this.cardData = cardData;
        cardInstanceId = Random.Range(0, 2000000000);

        //powers
        effectTriggers = new List<EffectTrigger>();
        tags = new();
        foreach (CardTag tag in cardData.tags) tags.Add(tag);
        containedCard = cardData.containedCard;

        modifiers = new();
    }

    //non gameplay elements
    public Sprite cardArt { get => cardData.cardArt; }
    public string title { get => cardData.title; }
    public string description { get => cardData.description; }

    //Action vs Unit
    public bool isAction { get => cardData.isAction; }

    //stats
    public int placementCost { get => cardData.placementCost; }
    public int iq { get => cardData.iq; }// private set
    public int health { get => cardData.health; }
    public int damageTaken { get; private set; }
    public int DamageTaken
    {
        get
        {
            return damageTaken;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            damageTaken = value;
        }
    }
    public int PlacementCost
    {
        get
        {
            int returnVal = placementCost;
            foreach (CardModifier mod in modifiers)
            {
                returnVal += mod.placementCost;
            }

            if (returnVal < 0)
            {
                returnVal = 0;
            }
            return returnVal;
        }
    }
    public int Iq
    {
        get
        {
            int returnVal = iq;
            foreach (CardModifier mod in modifiers)
            {
                returnVal += mod.iq;
            }

            if (returnVal < 0)
            {
                returnVal = 0;
            }

            return returnVal;
        }
    }
    public int Health
    {
        get
        {
            int returnVal = health;
            foreach (CardModifier mod in modifiers)
            {
                returnVal += mod.health;
            }
            returnVal -= damageTaken;

            if (returnVal < 0)
            {
                returnVal = 0;
            }

            return returnVal;
        }
    }

    //abilities
    public List<CardModifier> modifiers {get; set;}
    public bool noAttack { get => cardData.noAttack; }
    public List<EffectTrigger> effectTriggers {get; set;}
    public List<CardTag> tags {get; set;}
    public CardData containedCard { get; set; }

    //deck Creator
    public bool isInADeck { get; set; }

    public void PlacementAbility(ActionData actionData)// Card card
    {
        actionData.originCard = this;
        Debug.Log("placementCalled");

        foreach (EffectTrigger effectTrigger in effectTriggers)
        {
            if (effectTrigger != null)
            {
                effectTrigger.Placement(actionData);
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
    public void subET(ActionData actionData)
    {
        UnsubET();
        effectTriggers = new();
        foreach (EffectTriggerData effectTriggerData in cardData.effectTriggerDatas)
        {
            if (effectTriggerData != null)
            {
                EffectTrigger newEffectTrigger = new EffectTrigger(actionData, effectTriggerData);
                effectTriggers.Add(newEffectTrigger);
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
    public string GetTagList()
    {
        string tagList = "";
        bool first = true;
        foreach (CardTag tag in tags)
        {
            if (!first) tagList += ", ";
            tagList += tag.name;
            first = false;
        }
        return tagList;
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