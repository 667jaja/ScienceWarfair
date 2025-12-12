using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackLaneGA : GameAction
{
    public int playerId;
    public int lane;
    public int amount;
    public AttackLaneGA(int playerId2, int lane2, int amount2)
    {
        amount = amount2;
        playerId = playerId2;
        lane = lane2;
    }
}