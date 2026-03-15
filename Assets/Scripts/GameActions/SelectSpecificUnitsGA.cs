using System.Collections.Generic;
using UnityEngine;

public class SelectSpecificUnitsGA : GameAction
{
    public List<int> cardIds; //card instance Ids of selected units
    public bool newSelection;
    public SelectSpecificUnitsGA(List<int> cardIds2, bool newSelection2 = true)
    {
        cardIds = cardIds2;
        newSelection = newSelection2;
    }
}
