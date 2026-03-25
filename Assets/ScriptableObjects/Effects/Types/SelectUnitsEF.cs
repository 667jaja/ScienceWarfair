using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "SelectUnitsEffect", menuName = "Select Units Effect")]
public class SelectUnitsEF : Effect
{
    //selection
    [field: SerializeField] public CardTag targetTag { get; private set; }
    [field: SerializeField] public Vector2Int targetIqRange { get; private set; } = new Vector2Int (-1, -1);
    [field: SerializeField] public Vector2Int targetHealthRange { get; private set; } = new Vector2Int (-1, -1);
    [field: SerializeField] public Vector2Int targetPlacementCostRange { get; private set; } = new Vector2Int (-1, -1);

    [field: SerializeField] public int selectCount { get; private set; }
    [field: SerializeField] public bool includeFriendly { get; private set; } = true;
    [field: SerializeField] public bool includeEnemy { get; private set; } = true;
    [field: SerializeField] public bool includeSelf { get; private set; } = true;
    [field: SerializeField] public bool selectAll { get; private set; }
    [field: SerializeField] public int row { get; private set; } = -1;// a negative value means any row

    public override List<GameAction> effect
    {
        get
        {
            List<GameAction> actionList = new List<GameAction>(); 

            //selection
            List<Vector3Int> selectedThings = new();
            if (includeFriendly) selectedThings.AddRange(SelectionManager.instance.PositionsFromConditions(base.actionData.originPlayerId, row, targetTag, targetIqRange.x, targetIqRange.y, targetHealthRange.x, targetHealthRange.y, targetPlacementCostRange.x, targetPlacementCostRange.y));
            if (includeEnemy) selectedThings.AddRange(SelectionManager.instance.PositionsFromConditions(GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId), row, targetTag, targetIqRange.x, targetIqRange.y, targetHealthRange.x, targetHealthRange.y, targetPlacementCostRange.x, targetPlacementCostRange.y));
            if (!includeSelf && selectedThings.Contains(new Vector3Int(base.actionData.originPosition.x, base.actionData.originPosition.y, base.actionData.originPlayerId))) 
            selectedThings.Remove(new Vector3Int(base.actionData.originPosition.x, base.actionData.originPosition.y, base.actionData.originPlayerId));

            if (selectAll)
            {
                List<int> CardIds = new();
                foreach (Vector3Int vector in selectedThings)
                {
                    if (GameManager.instance.players[vector.z].units[vector.x, vector.y] != null) CardIds.Add(GameManager.instance.players[vector.z].units[vector.x, vector.y].cardInstanceId);
                }

                SelectSpecificUnitsGA selectSpecificUnitsGA = new SelectSpecificUnitsGA(CardIds);
                actionList.Add(selectSpecificUnitsGA);
            }
            else
            {
                SelectUnitsGA SelectunitsGA = new SelectUnitsGA(selectedThings, selectCount);
                actionList.Add(SelectunitsGA);
            }
            return actionList;
        }
    }  
}