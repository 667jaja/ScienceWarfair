using UnityEngine;

public class MoveUnitGA : GameAction
{
    public int playerId;
    public Vector2Int originPosition;
    public Vector2Int destinationPosition;

    public MoveUnitGA(int playerId2, Vector2Int originPosition2, Vector2Int destinationPosition2)
    {
        playerId = playerId2;
        originPosition = originPosition2;
        destinationPosition = destinationPosition2;
    }
}
