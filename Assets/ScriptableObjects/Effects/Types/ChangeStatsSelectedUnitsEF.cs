using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ChangeStatsSelectedUnitsEffect", menuName = "Change Stats Selected Units Effect")]
public class ChangeStatsSelectedUnitsEF : Effect
{
    //statchanges
    [field: SerializeField] public string modDescription { get; private set; }
    [field: SerializeField] public int iq { get; private set; }
    [field: SerializeField] public int health { get; private set; }
    [field: SerializeField] public int placementCost { get; private set; }
    [field: SerializeField] public EffectTriggerData et { get; private set; }
    [field: SerializeField] public List<CardTag> tags { get; private set; }
    [field: SerializeField] public bool isUnitedEffect { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            //stats
            ChangeStatsSelectedGA changeStatsSelectedGA = new ChangeStatsSelectedGA(modDescription, iq, health, placementCost, tags, (et == null)? -1 : et.effectTriggerId, null, isUnitedEffect);
            actionList.Add(changeStatsSelectedGA);

            return actionList;
        }
    }  
}