using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShotgunLaneEffect", menuName = "Shotgun Lane Effect")]
public class ShotgunAttackEF : Effect
{
    
    [field: SerializeField] public int attackDamage { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            int targetplayer = GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId);
            List<GameAction> actionList = new List<GameAction>(); 
            
            //animation
            if (base.specialAnimation != SpecialAnimation.Null)
            {
                SpecialAnimationGA specialAnimationGA = new SpecialAnimationGA(base.specialAnimation, new Vector2Int(base.actionData.originPosition.x,0), base.actionData.originPosition.x, GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId), SpecialAnimationManager.instance.AnimationLength(base.specialAnimation));
                actionList.Add(specialAnimationGA);
            } 

            AttackLaneShotgunGA attackLaneShotgunGA = new AttackLaneShotgunGA(targetplayer, base.actionData.originPosition.x, attackDamage);
            actionList.Add(attackLaneShotgunGA);

            return actionList;
        }
    }  

}

