using System.Collections.Generic;
using UnityEngine;

public class SelectLanesGA : GameAction
{
    public List<Vector2Int> validLanes; //x, playerId
    public int selectCount; 
    public SelectLanesGA(List<Vector2Int> validLanesInput, int selectCountInput)
    {
        validLanes = validLanesInput;
        selectCount = selectCountInput;
    }
}
