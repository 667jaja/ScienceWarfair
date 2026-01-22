using System.Collections.Generic;
using UnityEngine;

public class SelectUnitsGA : GameAction
{
    public List<Vector3Int> validLanes; //x, playerId
    public int selectCount; 
    public SelectUnitsGA(List<Vector3Int> validLanesInput, int selectCountInput)
    {
        validLanes = validLanesInput;
        selectCount = selectCountInput;
    }
}
