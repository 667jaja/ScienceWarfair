using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ChangeStatsSelectedUnitsEffect", menuName = "Change Stats Selected Units Effect")]
public class ChangeStatsSelectedUnitsEF : Effect
{
    [field: SerializeField] public int iq { get; private set; }
    [field: SerializeField] public int health { get; private set; }
    [field: SerializeField] public int placementCost { get; private set; }
    [field: SerializeField] public EffectTriggerData et { get; private set; }

    [field: SerializeField] public int selectCount { get; private set; }
    [field: SerializeField] public bool includeFriendly { get; private set; }
    [field: SerializeField] public bool includeEnemy { get; private set; }
    [field: SerializeField] public int row { get; private set; } // a negative value means any row

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            //selection
            List<Vector3Int> selectedThings = new();
            if (includeEnemy) selectedThings.AddRange(SelectionManager.instance.UnitsByPlayerRow(GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId), row));
            if (includeFriendly) selectedThings.AddRange(SelectionManager.instance.UnitsByPlayerRow(base.actionData.originPlayerId, row));
            
            SelectUnitsGA SelectunitsGA = new SelectUnitsGA(selectedThings, selectCount);
            actionList.Add(SelectunitsGA);

            //animation
            if (base.specialAnimation != SpecialAnimation.Null)
            {
                AnimateSelectedUnitsGA animateSelectedUnitsGA = new AnimateSelectedUnitsGA(base.specialAnimation, SpecialAnimationManager.instance.AnimationLength(base.specialAnimation)-0.2f, 0.2f);
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