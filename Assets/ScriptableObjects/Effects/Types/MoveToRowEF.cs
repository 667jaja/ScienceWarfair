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

            //animation
            if (base.specialAnimation != SpecialAnimation.Null)
            {
                SpecialAnimationGA specialAnimationGA = new SpecialAnimationGA(base.specialAnimation, new Vector2Int(base.actionData.originPosition.x, rowIndex), base.actionData.originPosition.x, base.actionData.originPlayerId, SpecialAnimationManager.instance.AnimationLength(base.specialAnimation));
                actionList.Add(specialAnimationGA);
            }

            MoveUnitGA moveUnitGA = new MoveUnitGA(base.actionData.originPlayerId, base.actionData.originPosition, new Vector2Int(base.actionData.originPosition.x, rowIndex));
            actionList.Add(moveUnitGA);

            return actionList;
        }
    }     
}
