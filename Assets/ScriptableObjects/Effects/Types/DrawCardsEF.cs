using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DrawCardsEffect", menuName = "Draw Cards Effect")]
public class DrawCardsEF : Effect
{
    
    [field: SerializeField] public int cardCount { get; private set; }

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

            DrawCardsGA drawCardsGA = new DrawCardsGA(base.actionData.originPlayerId, cardCount);
            actionList.Add(drawCardsGA);   
            
            return actionList;
        }
    }  


    // [field: SerializeField] public int placementCost { get; private set; }
    // [field: SerializeField] public int placementCost { get; private set; }
}
