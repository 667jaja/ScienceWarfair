using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackSelectedEffect", menuName = "Attack Selected Effect")]
public class AttackSelectedLaneEF : Effect
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

            AttackSelectedGA AttackselectedGA = new AttackSelectedGA(attackDamage);
            actionList.Add(AttackselectedGA);

            return actionList;
        }
    }  
}
