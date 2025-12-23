using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GainMoneyEffect", menuName = "Gain Money Effect")]
public class GainMoneyEF : Effect
{
    [field: SerializeField] public int gainCount { get; private set; }
    [field: SerializeField] public Card originCard { private get; set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 
            GainMoneyGA gainMoneyGA = new GainMoneyGA(base.actionData.playerId, gainCount);
            actionList.Add(gainMoneyGA);
            return actionList;
        }
    }  
}
