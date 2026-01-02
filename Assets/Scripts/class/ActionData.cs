using System.Collections.Generic;
using UnityEngine;

public class ActionData
{
    public int originPlayerId;
    public Card originCard;
    public Vector2Int originPosition;

    public int targetPlayerId;
    public List<Vector2Int> targetPositions;
    public List<Card> targetHandCards, targetDiscardCards, targetDeckCards;
    public Card targetCard;
    
    public ActionData(int originPlayerId1, Vector2Int originPosition1, Card originCard1)
    {
        originPlayerId = originPlayerId1;
        originPosition = originPosition1;
        originCard = originCard1;
        targetPlayerId = GameManager.instance.GetNextPlayerId(originPlayerId1);
    }
}
