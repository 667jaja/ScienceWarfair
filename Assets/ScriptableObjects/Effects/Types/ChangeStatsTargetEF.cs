using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChangeStatsTargetEffect", menuName = "Change Stats Target Effect")]
public class ChangeStatsTargetEF : Effect
{
    [field: SerializeField] public int iq { get; private set; }
    [field: SerializeField] public int health { get; private set; }
    [field: SerializeField] public int placementCost { get; private set; }
    [field: SerializeField] public EffectTriggerData et { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 
            
            SelectSpecificUnitsGA selectSpecificUnits = new SelectSpecificUnitsGA(new List<int>(){base.actionData.targetCard.cardInstanceId}, true);
            actionList.Add(selectSpecificUnits);
    
            //animation
            if (base.specialAnimation != SpecialAnimation.Null)
            {
                AnimateSelectedUnitsGA animateSelectedUnitsGA = new AnimateSelectedUnitsGA(base.specialAnimation,SpecialAnimationManager.instance.AnimationLength(base.specialAnimation)-0.2f, 0.2f);
                actionList.Add(animateSelectedUnitsGA);
            }
            
            //stats
            if (iq> 0 || health > 0 || placementCost > 0)
            {
                ChangeStatsSelectedGA changeStatsSelectedGA = new ChangeStatsSelectedGA(iq, health, placementCost);
                actionList.Add(changeStatsSelectedGA);
            }
            //effects
            if (et != null)
            {
                GiveSelectedEffectGA giveSelectedEffectGA = new GiveSelectedEffectGA(et);
                actionList.Add(giveSelectedEffectGA);
            }
            return actionList;
        }
    }  
}
