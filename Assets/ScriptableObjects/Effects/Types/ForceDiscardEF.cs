using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ForceDiscardEffect", menuName = "Force Discard Effect")]
public class ForceDiscardEF : Effect
{
    [field: SerializeField] public int selectCount { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            //selection
            SelectCardsGA selectCardsGA = new SelectCardsGA(GameManager.instance.players[base.actionData.originPlayerId].hand, selectCount);
            actionList.Add(selectCardsGA);

            //animation
            if (base.specialAnimation != SpecialAnimation.Null)
            {
                SpecialAnimationGA specialAnimationGA = new SpecialAnimationGA(base.specialAnimation, new Vector2Int(base.actionData.originPosition.x, base.actionData.originPosition.y), base.actionData.originPosition.x, base.actionData.originPlayerId, SpecialAnimationManager.instance.AnimationLength(base.specialAnimation));
                actionList.Add(specialAnimationGA);
            }

            DiscardSelectedGA discardSelectedGA = new DiscardSelectedGA(base.actionData.originPlayerId);
            actionList.Add(discardSelectedGA);


    
            return actionList;
        }
    }  
}
