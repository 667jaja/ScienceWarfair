using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveToRowEffect", menuName = "Move To Row Effect")]
public class MoveToRowEF : Effect
{
    [field: SerializeField] public int rowIndex { get; private set; }
    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            MoveUnitGA moveUnitGA = new MoveUnitGA(base.actionData.originPlayerId, base.actionData.originPosition, new Vector2Int(base.actionData.originPosition.x, rowIndex));
            actionList.Add(moveUnitGA);

            return actionList;
        }
    }     
}
