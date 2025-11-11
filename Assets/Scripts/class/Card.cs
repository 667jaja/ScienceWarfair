using Unity.VisualScripting;
using UnityEngine;

public class Card
{
    private readonly CardData cardData;
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

    //stats
    public int placementCost { get; set; }
    public int iq { get; set; }
    public int health { get; set; }

    //abilities
    public string cardEffect { get; set; }

    public void PerformEffect()
    {
        Debug.Log("used ability " + cardEffect);
    }
}