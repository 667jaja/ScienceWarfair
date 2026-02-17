using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackAllLanesEffect", menuName = "Attack All Lanes Effect")]
public class AttackAllLanesEF : Effect
{
    [field: SerializeField] public int attackDamage { get; private set; }
    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 
            for (int i = 0; i < UnitManager.instance.columnCount; i++)
            {
                AttackLaneGA attackLaneGA = new AttackLaneGA(GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId), i,  attackDamage);
                actionList.Add(attackLaneGA);
            }

            return actionList;
        }
    }
}
