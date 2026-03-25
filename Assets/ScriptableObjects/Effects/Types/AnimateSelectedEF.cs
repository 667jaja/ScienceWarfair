using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "AnimateSelectedEffect", menuName = "Animate Selected Effect")]
public class animateSelectedEF : Effect
{
    [field: SerializeField] public bool units { get; private set; }
    [field: SerializeField] public bool lanes { get; private set; }
    [field: SerializeField] public float animDelay { get; private set; } = .2f;

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            //animation
            if (base.specialAnimation != SpecialAnimation.Null && units)
            {
                AnimateSelectedUnitsGA animateSelectedUnitsGA = new AnimateSelectedUnitsGA(base.specialAnimation, SpecialAnimationManager.instance.AnimationLength(base.specialAnimation)-animDelay, animDelay);
                actionList.Add(animateSelectedUnitsGA);
            }
            //animation
            if (base.specialAnimation != SpecialAnimation.Null && lanes)
            {
                AnimateSelectedLanesGA animateSelectedLanesGA = new AnimateSelectedLanesGA(base.specialAnimation,SpecialAnimationManager.instance.AnimationLength(base.specialAnimation)-animDelay, animDelay);
                actionList.Add(animateSelectedLanesGA);
            }

            return actionList;
        }
    }  
}