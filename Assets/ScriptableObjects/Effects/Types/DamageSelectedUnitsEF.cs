using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageSelectedUnitsEffect", menuName = "Damage Selected Units Effect")]
public class DamageSelectedUnitsEF : Effect
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
                AnimateSelectedUnitsGA animateSelectedUnitsGA = new AnimateSelectedUnitsGA(base.specialAnimation,SpecialAnimationManager.instance.AnimationLength(base.specialAnimation)-0.2f, 0.2f);
                actionList.Add(animateSelectedUnitsGA);
            }

            DamageSelectedGA damageSelectedGA = new DamageSelectedGA(attackDamage);
            actionList.Add(damageSelectedGA);
            

            return actionList;
        }
    }  
}