using UnityEngine;

public class PlaceUnitGA : GameAction
{
    public int playerId;
    public int lane;
    public Card playedCard;
    public PlaceUnitGA(int playerIdInput, int laneInput, Card cardInput)
    {
        base.isInputAction = true;
        playerId = playerIdInput;
        lane = laneInput;
        playedCard = cardInput;
    }
}
