using UnityEngine;

public class CreateUnitGA : GameAction
{
    public int playerId;
    public int lane;
    public Card playedCard;
    public CreateUnitGA(int playerIdInput, int laneInput, Card cardInput)
    {
        playerId = playerIdInput;
        lane = laneInput;
        playedCard = cardInput;
    }
}
