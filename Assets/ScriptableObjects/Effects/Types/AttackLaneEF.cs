using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackLaneEffect", menuName = "Attack Lane Effect")]
public class AttackLaneEF : Effect
{
    
    [field: SerializeField] public int attackDamage { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            //animation
            if (base.specialAnimation != SpecialAnimation.Null)
            {
                SpecialAnimationGA specialAnimationGA = new SpecialAnimationGA(base.specialAnimation, new Vector2Int(base.actionData.originPosition.x,0), base.actionData.originPosition.x, GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId), SpecialAnimationManager.instance.AnimationLength(base.specialAnimation));
                actionList.Add(specialAnimationGA);
            }

            AttackLaneGA attackLaneGA = new AttackLaneGA(GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId), base.actionData.originPosition.x,  attackDamage);
            actionList.Add(attackLaneGA);
            
            return actionList;
        }
    }  
}
