using UnityEngine;

public class BoardToHandGA : GameAction
{
    public int recieverId;
    public Vector3Int position;
    public bool clone;

    public BoardToHandGA(int recieverId2, Vector3Int position2, bool clone2 = false)
    {
        recieverId = recieverId2;
        position = position2;
        clone = clone2;
    }
}
