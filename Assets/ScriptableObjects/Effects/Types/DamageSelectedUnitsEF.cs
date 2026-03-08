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

            List<Vector3Int> selectedThings = new();
            if (includeEnemy) selectedThings.AddRange(SelectionManager.instance.UnitsByPlayerRow(GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId), row));
            if (includeFriendly) selectedThings.AddRange(SelectionManager.instance.UnitsByPlayerRow(base.actionData.originPlayerId, row));
            
            SelectUnitsGA SelectunitsGA = new SelectUnitsGA(selectedThings, selectCount);
            actionList.Add(SelectunitsGA);

            DamageSelectedGA damageSelectedGA = new DamageSelectedGA(attackDamage);
            actionList.Add(damageSelectedGA);

            return actionList;
        }
    }  
}