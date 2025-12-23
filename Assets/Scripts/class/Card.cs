using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Card
{
    private CardData cardData;
    public Card(CardData cardData)
    {
        this.cardData = cardData;
        cardEffect = cardData.cardEffect;
        placementCost = cardData.placementCost;
        iq = cardData.iq;
        health = cardData.health;
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
    public string cardEffect { get; set; }
    public List<Effect> effects {get => cardData.effects;}

    public void PlacementAbility(ActionData actionData)// Card card
    {
        PerformEffect(actionData);
    }
    public void PerformEffect(ActionData actionData)
    {
        Debug.Log("used ability " + cardEffect);
        //foreach 
        if (effects.Count > 0 && effects[0] != null)
        {
            cardData.effects[0].actionData = actionData;
            ActionManager.instance.Perform(effects[0].effect);
        }
    }
}