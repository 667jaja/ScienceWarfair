using UnityEngine;

public class GainActionPointsGA : GameAction
{    
    public int playerId;
    public int gainCount;
    public GainActionPointsGA(int playerId1, int gainCount1)
    {
        playerId = playerId1;
        gainCount = gainCount1;
    }
}
