using UnityEngine;

public class PlayActionGA : GameAction
{
    public int playerId;
    public Card playedCard;
    public PlayActionGA(int playerIdInput, Card cardInput)
    {
        base.isInputAction = true;
        playerId = playerIdInput;
        playedCard = cardInput;
    }
}
