using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Numerics;

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

    //selection
    [field: SerializeField] public CardTag targetTag { get; private set; }
    [field: SerializeField] public int selectCount { get; private set; }
    [field: SerializeField] public bool includeFriendly { get; private set; }
    [field: SerializeField] public bool includeEnemy { get; private set; }
    [field: SerializeField] public bool includeSelf { get; private set; }
    [field: SerializeField] public bool selectAll { get; private set; }
    [field: SerializeField] public int row { get; private set; } // a negative value means any row

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            //selection
            List<Vector3Int> selectedThings = new();
            if (includeEnemy) selectedThings.AddRange(SelectionManager.instance.PositionsFromConditions(GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId), row, targetTag));
            if (includeFriendly) selectedThings.AddRange(SelectionManager.instance.PositionsFromConditions(base.actionData.originPlayerId, row, targetTag));
            if (!includeSelf && selectedThings.Contains(new Vector3Int(base.actionData.originPosition.x, base.actionData.originPosition.y, base.actionData.originPlayerId))) 
            selectedThings.Remove(new Vector3Int(base.actionData.originPosition.x, base.actionData.originPosition.y, base.actionData.originPlayerId));

            if (selectAll)
            {
                List<int> CardIds = new();
                foreach (Vector3Int vector in selectedThings)
                {
                    if (GameManager.instance.players[vector.z].units[vector.x, vector.y] != null) CardIds.Add(GameManager.instance.players[vector.z].units[vector.x, vector.y].cardInstanceId);
                }

                SelectSpecificUnitsGA selectSpecificUnitsGA = new SelectSpecificUnitsGA(CardIds);
                actionList.Add(selectSpecificUnitsGA);
            }
            else
            {
                SelectUnitsGA SelectunitsGA = new SelectUnitsGA(selectedThings, selectCount);
                actionList.Add(SelectunitsGA);
            }


            //animation
            if (base.specialAnimation != SpecialAnimation.Null)
            {
                AnimateSelectedUnitsGA animateSelectedUnitsGA = new AnimateSelectedUnitsGA(base.specialAnimation, SpecialAnimationManager.instance.AnimationLength(base.specialAnimation)-0.2f, 0.2f);
                actionList.Add(animateSelectedUnitsGA);
            }

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