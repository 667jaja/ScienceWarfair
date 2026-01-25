using UnityEngine;

public class ChangeStatsUnitGA : GameAction
{
    // public unit target
    public int playerId;
    public Vector2Int position;
    public int iqChange, heathChange, costChange;
    public ChangeStatsUnitGA(int playerId2, Vector2Int position2, int iqChange2, int heathChange2, int costChange2)
    {
        playerId = playerId2;
        position = position2;
        iqChange = iqChange2;
        heathChange = heathChange2;
        costChange = costChange2;
    }
}
