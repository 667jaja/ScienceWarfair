using UnityEngine;

public class Card
{
    private readonly CardData cardData;
    public Card(CardData cardData)
    {
        this.cardData = cardData;
        cardEffect = cardData.cardEffect;
        placementCost = cardData.placementCost;
    }
    public Sprite cardArt { get => cardData.cardArt; }
    public string title { get => cardData.title; }
    public int placementCost { get; set; }
    public string cardEffect { get; set; }

    public void PerformEffect()
    {
        Debug.Log("used ability " + cardEffect);
    }
}