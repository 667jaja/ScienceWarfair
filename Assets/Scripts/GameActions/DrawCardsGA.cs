using UnityEngine;

public class DrawCardsGA : GameAction
{
    public int playerId;
    public int drawCount;
    public DrawCardsGA(int playerId1, int drawCount1)
    {
        playerId = playerId1;
        drawCount = drawCount1;
    }
}
