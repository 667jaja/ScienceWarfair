using UnityEngine;

public class DeckBuilderDropArea : MonoBehaviour, ICardDropArea
{
    public bool isAllCards;
    public void OnCardDrop(Card card)
    {
        if (isAllCards)
        {
            DeckBuilderManager.instance.RemoveCardFromCurrentDeck(card.cardData);
        }
        else
        {
            DeckBuilderManager.instance.AddCardToCurrentDeck(card.cardData);
        }
    }
}
