using UnityEngine;

public class IncreaseStatsGA : GameAction
{
    // public unit target
    public int attackIncreaseAmount, heathIncreaseAmount;
    public IncreaseStatsGA(int attackIncreaseAmount2, int heathIncreaseAmount2)
    {
        attackIncreaseAmount = attackIncreaseAmount2;
        heathIncreaseAmount = heathIncreaseAmount2;
    }
}
