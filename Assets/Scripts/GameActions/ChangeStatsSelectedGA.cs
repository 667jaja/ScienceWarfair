using UnityEngine;

public class ChangeStatsSelectedGA : GameAction
{
    public int iqChange, heathChange, costChange;
    public ChangeStatsSelectedGA(int iqChange2, int heathChange2, int costChange2)
    {
        iqChange = iqChange2;
        heathChange = heathChange2;
        costChange = costChange2;
    }
}
