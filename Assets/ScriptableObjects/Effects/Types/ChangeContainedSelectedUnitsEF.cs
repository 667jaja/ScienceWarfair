using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ChangeContainedSelectedUnitsEffect", menuName = "Change Contained Selected Units Effect")]
public class ChangeContainedSelectedUnitsEF : Effect
{
    [field: SerializeField] public CardData containedCard { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            //stats
            ChangeContainedSelectedGA changeContainedSelectedGA = new ChangeContainedSelectedGA(containedCard);
            actionList.Add(changeContainedSelectedGA);

            return actionList;
        }
    }  
}