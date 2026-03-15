using UnityEngine;

public class SpecialAnimationGA : GameAction
{
    public SpecialAnimation animVal;
    public Vector2Int unitLocation;
    public int laneLocation;
    public int playerId;
    public float waitTime;
    public SpecialAnimationGA(SpecialAnimation animVal2, Vector2Int unitLocation2, int laneLocation2, int playerId2, float waitTime2)
    {
        animVal = animVal2;
        unitLocation = unitLocation2;
        laneLocation = laneLocation2;
        playerId = playerId2;
        waitTime = waitTime2;
    }
}
