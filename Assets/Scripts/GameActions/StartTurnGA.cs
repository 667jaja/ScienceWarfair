using UnityEngine;

public class StartTurnGA : GameAction
{
    public int playerId;
    public StartTurnGA(int playerIdInput)
    {
        playerId = playerIdInput;
    }
}
