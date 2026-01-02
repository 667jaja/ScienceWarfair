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

            DrawCardsGA drawCardsGA = new DrawCardsGA(base.actionData.originPlayerId, cardCount);
            actionList.Add(drawCardsGA);
            return actionList;
        }
    }  


    // [field: SerializeField] public int placementCost { get; private set; }
    // [field: SerializeField] public int placementCost { get; private set; }
}
