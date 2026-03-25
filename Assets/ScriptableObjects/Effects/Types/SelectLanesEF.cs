using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectLanesEffect", menuName = "Select Lanes Effect")]
public class SelectLanesEF : Effect
{
    [field: SerializeField] public int selectCount { get; private set; }
    [field: SerializeField] public bool alsoOpposingLanes { get; private set; }
    [field: SerializeField] public bool unitsInLanes { get; private set; }

    [field: SerializeField] public bool includeFriendly { get; private set; } = true;
    [field: SerializeField] public bool includeEnemy { get; private set; } = true;
    [field: SerializeField] public bool includeSelf { get; private set; } = true;
    [field: SerializeField] public bool includeOpp { get; private set; } = true;

//    [field: SerializeField] public bool selectAll { get; private set; }
    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            //selection
            List<Vector2Int> selectedThings = new();

            //conditions
            if (includeFriendly) selectedThings.AddRange(SelectionManager.instance.LanesFromConditions(base.actionData.originPlayerId));
            if (includeEnemy) selectedThings.AddRange(SelectionManager.instance.LanesFromConditions(GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId)));

            if (!includeSelf && selectedThings.Contains(new Vector2Int(base.actionData.originPosition.x, base.actionData.originPlayerId))) 
            selectedThings.Remove(new Vector2Int(base.actionData.originPosition.x, base.actionData.originPlayerId));
            if (!includeOpp && selectedThings.Contains(new Vector2Int(base.actionData.originPosition.x, GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId)))) 
            selectedThings.Remove(new Vector2Int(base.actionData.originPosition.x, GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId)));

            //select
            SelectLanesGA selectLanesGA = new SelectLanesGA(selectedThings,selectCount);
            actionList.Add(selectLanesGA);

            if (alsoOpposingLanes) actionList.Add(new SelectOpposingLanesGA());
            
            if (unitsInLanes) actionList.Add(new SelectUnitsInLanesGA());

            return actionList;
        }
    }  
}
