using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Card
{
    private CardData cardData;

    //constructor
    public Card(CardData cardData)
    {
        this.cardData = cardData;
        cardEffect = cardData.cardEffect;
        placementCost = cardData.placementCost;
        iq = cardData.iq;
        health = cardData.health;
        noAttack = cardData.noAttack;
        effectTriggers = new List<EffectTrigger>() {cardData.effectTriggers};
        effects = cardData.effects;
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

    //abilities
    public bool noAttack { get; set; }
    public string cardEffect { get; set; }
    public List<EffectTrigger> effectTriggers {get; set;}
    public List<Effect> effects {get; set;}
    public CardData containedCard { get; set; }

    public void PlacementAbility(ActionData actionData)// Card card
    {
        actionData.originCard = this;
        Debug.Log("placementCalled");
        if (effectTriggers.Count > 0)
        {
            foreach (EffectTrigger effectTrigger in effectTriggers)
            {
                if (effectTrigger != null)
                {
                    effectTrigger.Placement(actionData);
                }
            }
        }     
    }
    public void PerformAbility(ActionData actionData)
    {
        Debug.Log("performAbilityCalled");
        if (effects.Count > 0)
        {
            foreach (Effect effect in effects)
            {
                if (effect != null)
                {
                    effect.actionData = actionData;
                    foreach (GameAction action in effect.effect)
                    {
                        ActionManager.instance.Perform(action);
                    }
                }
            }
        }
    }
    public void Destruciton()
    {
        if (effectTriggers.Count > 0)
        {
            foreach (EffectTrigger effectTrigger in effectTriggers)
            {
                if (effectTrigger != null)
                {
                    effectTrigger.DestructionEffect();
                }
            }
        }     

    }
}