using UnityEngine;

public class LaneDropArea : MonoBehaviour, ICardDropArea
{
    public int laneId;
    public bool isActionLane;
    public void OnCardDrop(Card card)
    {
        int currentPlayer = GameManager.instance.currentPlayer;
    
        // Debug.Log("player id: " + currentPlayer);
        // Debug.Log("lane id: " + laneId);
        // Debug.Log("card name: " + card.title);

        if (currentPlayer == GameManager.instance.displayPlayer)
        {
            if (!isActionLane)
            {
                UnitManager.instance.PlaceCard(currentPlayer, laneId, card);
            }
            else
            {
                UnitManager.instance.PlayAction(currentPlayer, card);
            }
        }
    }
}
