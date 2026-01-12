using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GainSciencePointsEffect", menuName = "Gain Science Points Effect")]
public class GainSciencePointsEF : Effect
{
    [field: SerializeField] public int gainCount { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 
            GainSciencePointsGA gainSciencePointsGA = new GainSciencePointsGA(base.actionData.originPlayerId, gainCount);
            actionList.Add(gainSciencePointsGA);
            return actionList;
        }
    }  
}