using UnityEngine;

public class BoardToHandGA : GameAction
{
    public int recieverId;
    public Vector3Int position;

    public BoardToHandGA(int recieverId2, Vector3Int position2)
    {
        recieverId = recieverId2;
        position = position2;
    }
}
