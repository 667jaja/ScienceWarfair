using UnityEngine;

public class DiscardCardGA : GameAction
{
    public int playerId;
    public Card discardedCard;
    public DiscardCardGA(int playerIdInput, Card cardInput)
    {
        //base.isInputAction = true;
        playerId = playerIdInput;
        discardedCard = cardInput;
    }
}
