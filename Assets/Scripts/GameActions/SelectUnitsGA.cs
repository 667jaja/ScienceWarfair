using System.Collections.Generic;
using UnityEngine;

public class SelectUnitsGA : GameAction
{
    public List<Vector3Int> validPos; //x, y, playerId
    public int selectCount; 
    public SelectUnitsGA(List<Vector3Int> validPosInput, int selectCountInput)
    {
        validPos = validPosInput;
        selectCount = selectCountInput;
    }
}
