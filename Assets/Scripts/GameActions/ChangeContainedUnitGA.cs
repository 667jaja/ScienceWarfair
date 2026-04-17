using UnityEngine;

public class ChangeContainedUnitGA : GameAction
{
    public int playerId;
    public Vector2Int position;
    public CardData newContainedCard;
    public ChangeContainedUnitGA(int playerId2, Vector2Int position2, CardData newContainedCard2)
    {
        playerId = playerId2;
        position = position2;
        newContainedCard = newContainedCard2;
    }
}
