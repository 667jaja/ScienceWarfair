using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ChangeStatsSelectedUnitsEffect", menuName = "Change Stats Selected Units Effect")]
public class ChangeStatsSelectedUnitsEF : Effect
{
    //statchanges
    [field: SerializeField] public int iq { get; private set; }
    [field: SerializeField] public int health { get; private set; }
    [field: SerializeField] public int placementCost { get; private set; }
    [field: SerializeField] public EffectTriggerData et { get; private set; }
    [field: SerializeField] public CardTag tag { get; private set; }
    [field: SerializeField] public bool isUnitedEffect { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            //stats
            if (iq != 0 || health != 0 || placementCost != 0 || tag != null)
            {
                ChangeStatsSelectedGA changeStatsSelectedGA = new ChangeStatsSelectedGA(iq, health, placementCost, tag);
                actionList.Add(changeStatsSelectedGA);
            }

            //effects
            if (et != null && !isUnitedEffect)
            {
                GiveSelectedEffectGA giveSelectedEffectGA = new GiveSelectedEffectGA(et);
                actionList.Add(giveSelectedEffectGA);
            }
            if (et != null && isUnitedEffect)
            {
                GiveSelectedUnitedEffectGA giveSelectedEffectGA = new GiveSelectedUnitedEffectGA(et);
                actionList.Add(giveSelectedEffectGA);
            }

            return actionList;
        }
    }  
}