using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "SelectedUnitsToHandEffect", menuName = "Selected Units To Hand Effect")]
public class SelectedUnitsToHandEF : Effect
{
    //statchanges
    [field: SerializeField] public bool ownersHand;
    [field: SerializeField] public bool refundOwner;
    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            BoardToHandSelectedGA BoardToHandSelectedGA = new BoardToHandSelectedGA(base.actionData.originPlayerId, ownersHand, refundOwner);
            actionList.Add(BoardToHandSelectedGA);


            return actionList;
        }
    }  
}