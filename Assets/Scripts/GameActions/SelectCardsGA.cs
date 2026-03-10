using System.Collections.Generic;
using UnityEngine;

public class SelectCardsGA : GameAction
{
    public List<Card> validCards; //x, y, playerId
    public int selectCount; 
    public SelectCardsGA(List<Card> validCardsInput, int selectCountInput)
    {
        validCards = validCardsInput;
        selectCount = selectCountInput;
    }
}
