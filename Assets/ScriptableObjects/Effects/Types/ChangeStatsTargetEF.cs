using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChangeStatsTargetEffect", menuName = "Change Stats Target Effect")]
public class ChangeStatsTargetEF : Effect
{
    [field: SerializeField] public int iq { get; private set; }
    [field: SerializeField] public int health { get; private set; }
    [field: SerializeField] public int placementCost { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            ChangeStatsUnitGA changeStatsUnitGA = new ChangeStatsUnitGA(base.actionData.targetPlayerId, base.actionData.targetPositions[0], iq, health, placementCost);
            actionList.Add(changeStatsUnitGA);

            return actionList;
        }
    }  
}
