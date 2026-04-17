using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChangeStatsTargetEffect", menuName = "Change Stats Target Effect")]
public class ChangeStatsTargetEF : Effect
{
    //stats
    [field: SerializeField] public string modDescription { get; private set; }
    [field: SerializeField] public int iq { get; private set; }
    [field: SerializeField] public int health { get; private set; }
    [field: SerializeField] public int placementCost { get; private set; }
    [field: SerializeField] public EffectTriggerData et { get; private set; }
    [field: SerializeField] public List<CardTag> tags { get; private set; }

    //target
    [field: SerializeField] public bool targetSaved { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            Card targetCard = (targetSaved)? base.savedCard : base.actionData.targetCard;

            if (targetCard != null)
            {
                int targetCardInstanceId = targetCard.cardInstanceId;
                
                SelectSpecificUnitsGA selectSpecificUnits = new SelectSpecificUnitsGA(new List<int>(){targetCardInstanceId}, true);
                actionList.Add(selectSpecificUnits);
        
                //animation
                if (base.specialAnimation != SpecialAnimation.Null)
                {
                    AnimateSelectedUnitsGA animateSelectedUnitsGA = new AnimateSelectedUnitsGA(base.specialAnimation,SpecialAnimationManager.instance.AnimationLength(base.specialAnimation)-0.2f, 0.2f);
                    actionList.Add(animateSelectedUnitsGA);
                }
                
                //stats
                ChangeStatsSelectedGA changeStatsSelectedGA = new ChangeStatsSelectedGA(modDescription, iq, health, placementCost, tags, (et == null)? -1 : et.effectTriggerId);
                actionList.Add(changeStatsSelectedGA);
            }
            return actionList;
        }
    }  
}
