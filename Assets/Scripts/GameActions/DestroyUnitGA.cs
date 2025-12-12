using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyUnitGA : GameAction
{
    public int playerId;
    public Vector2Int position;
    public DestroyUnitGA(int playerId2, Vector2Int position2)
    {
        playerId = playerId2;
        position = position2;
    }
}