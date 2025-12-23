
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GainActionPointsEffect", menuName = "Gain Action Points Effect")]
public class GainActionPointsEF : Effect
{
    [field: SerializeField] public int gainCount { get; private set; }
    [field: SerializeField] public Card originCard { private get; set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 
            GainActionPointsGA gainActionPointsGA = new GainActionPointsGA(base.actionData.playerId, gainCount);
            actionList.Add(gainActionPointsGA);
            return actionList;
        }
    }  
}
