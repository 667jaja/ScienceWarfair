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

            SelectCardsGA selectCardsGA = new SelectCardsGA(GameManager.instance.players[base.actionData.originPlayerId].hand, selectCount);
            actionList.Add(selectCardsGA);

            DiscardSelectedGA discardSelectedGA = new DiscardSelectedGA(base.actionData.originPlayerId);
            actionList.Add(discardSelectedGA);

            return actionList;
        }
    }  
}
