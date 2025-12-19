using UnityEngine;

public class ActionData
{
    public int playerId;
    public Vector2Int position;
    public Card card;
    
    public ActionData(int playerId1, Vector2Int position1, Card card1)
    {
        playerId = playerId1;
        position = position1;
        card = card1;
    }
}
