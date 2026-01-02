using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GainMoneyEffect", menuName = "Gain Money Effect")]
public class GainMoneyEF : Effect
{
    [field: SerializeField] public int gainCount { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 
            GainMoneyGA gainMoneyGA = new GainMoneyGA(base.actionData.originPlayerId, gainCount);
            actionList.Add(gainMoneyGA);
            return actionList;
        }
    }  
}
