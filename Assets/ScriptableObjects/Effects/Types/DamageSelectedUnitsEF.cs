using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageSelectedUnitsEffect", menuName = "Damage Selected Units Effect")]
public class DamageSelectedUnitsEF : Effect
{
    [field: SerializeField] public int attackDamage { get; private set; }
    [field: SerializeField] public int selectCount { get; private set; }
    [field: SerializeField] public bool includeFriendly { get; private set; }
    [field: SerializeField] public bool includeEnemy { get; private set; }
    [field: SerializeField] public int row { get; private set; } // a negative value means any row

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            if (includeEnemy)
            {
                SelectUnitsGA SelectunitsGA = new SelectUnitsGA(SelectionManager.instance.UnitsByPlayerRow(GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId), row), selectCount);
                actionList.Add(SelectunitsGA);
            }
            if (includeFriendly)
            {
                SelectUnitsGA SelectunitsGA = new SelectUnitsGA(SelectionManager.instance.UnitsByPlayerRow(base.actionData.originPlayerId, row), selectCount);
                actionList.Add(SelectunitsGA);
            }

            DamageSelectedGA damageSelectedGA = new DamageSelectedGA(attackDamage);
            actionList.Add(damageSelectedGA);

            return actionList;
        }
    }  
}