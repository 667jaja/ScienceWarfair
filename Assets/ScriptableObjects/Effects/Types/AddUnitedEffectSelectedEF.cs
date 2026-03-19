using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddUnitedETSelectedEffect", menuName = "Add United ET Selected Effect")]
public class AddUnitedEffectSelectedEF : Effect
{
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
