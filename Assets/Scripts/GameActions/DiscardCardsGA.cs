using System.Collections.Generic;
using UnityEngine;

public class DiscardCardsGA : GameAction
{
    public int playerId;
    public List<Card> discardedCards;
    public DiscardCardsGA(int playerIdInput, List<Card> cardsInput)
    {
        //base.isInputAction = true;
        playerId = playerIdInput;
        discardedCards = cardsInput;
    }
}
