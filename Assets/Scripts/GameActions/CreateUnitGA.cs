using UnityEngine;

public class CreateUnitGA : GameAction
{
    public int playerId;
    public Vector2Int position;
    public Card playedCard;
    public CreateUnitGA(int playerIdInput, Vector2Int positionInput, Card cardInput)
    {
        playerId = playerIdInput;
        position = positionInput;
        playedCard = cardInput;
    }
}
