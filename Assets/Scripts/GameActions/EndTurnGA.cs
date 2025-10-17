using UnityEngine;

public class EndTurnGA : GameAction
{
    public int playerId;
    public EndTurnGA(int playerIdInput)
    {
        playerId = playerIdInput;
    }
}
