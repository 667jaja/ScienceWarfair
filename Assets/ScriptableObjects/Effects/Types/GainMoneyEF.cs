using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GainMoneyEffect", menuName = "Gain Money Effect")]
public class GainMoneyEF : Effect
{
    [field: SerializeField] public int gainCount { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 
                        
            //animation
            if (base.specialAnimation != SpecialAnimation.Null)
            {
                SpecialAnimationGA specialAnimationGA = new SpecialAnimationGA(base.specialAnimation, new Vector2Int(base.actionData.originPosition.x, base.actionData.originPosition.y), base.actionData.originPosition.x, base.actionData.originPlayerId, SpecialAnimationManager.instance.AnimationLength(base.specialAnimation));
                actionList.Add(specialAnimationGA);
            }
            
            GainMoneyGA gainMoneyGA = new GainMoneyGA(base.actionData.originPlayerId, gainCount);
            actionList.Add(gainMoneyGA);

            return actionList;
        }
    }  
}
