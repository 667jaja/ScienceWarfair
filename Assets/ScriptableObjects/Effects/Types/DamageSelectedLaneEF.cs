using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageSelectedLaneEffect", menuName = "Damage Selected Lane Effect")]
public class DamageSelectedLaneEF : Effect
{
    [field: SerializeField] public int attackDamage { get; private set; }
    [field: SerializeField] public int selectCount { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            SelectLanesGA selectLanesGA = new SelectLanesGA(SelectionManager.instance.EnemyLanes(base.actionData.originPlayerId),selectCount);
            actionList.Add(selectLanesGA);

            actionList.Add(new SelectUnitsInLanesGA());

            DamageSelectedGA damageSelectedGA = new DamageSelectedGA(attackDamage);
            actionList.Add(damageSelectedGA);

            return actionList;
        }
    }  
}
