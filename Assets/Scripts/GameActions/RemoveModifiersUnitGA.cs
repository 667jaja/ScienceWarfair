using UnityEngine;

public class RemoveModifiersUnitGA : GameAction
{
    public int playerId;
    public Vector2Int position;
    public RemoveModifiersUnitGA(int playerId2, Vector2Int position2)
    {
        playerId = playerId2;
        position = position2;
    }
}
