using UnityEngine;

public class PlayActionGA : GameAction
{
    public int playerId;
    public Card playedCard;
    public PlayActionGA(int playerIdInput, Card cardInput)
    {
        playerId = playerIdInput;
        playedCard = cardInput;
    }
}
