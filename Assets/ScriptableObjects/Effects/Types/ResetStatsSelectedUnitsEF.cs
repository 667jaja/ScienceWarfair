using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ResetStatsSelectedUnitsEffect", menuName = "Reset Stats Selected Units Effect")]
public class ResetStatsSelectedUnitsEF : Effect
{
    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            //stats
            RemoveModifiersSelectedGA removeModifiersSelectedGA = new RemoveModifiersSelectedGA();
            actionList.Add(removeModifiersSelectedGA);

            return actionList;
        }
    }  
}