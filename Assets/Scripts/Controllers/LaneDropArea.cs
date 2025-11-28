using UnityEngine;

public class LaneDropArea : MonoBehaviour, ICardDropArea
{
    [SerializeField] private int laneId;
    public void OnCardDrop(Card card)
    {
        int currentPlayer = GameManager.instance.currentPlayer;
        int displayPlayer = GameManager.instance.displayPlayer;
    
        // Debug.Log("player id: " + currentPlayer);
        // Debug.Log("lane id: " + laneId);
        // Debug.Log("card name: " + card.title);
        // card.transform.parent = this.transform;
        // card.transform.position = Vector3.zero;
        if (currentPlayer == displayPlayer )
        {
            UnitManager.instance.PlaceCard(currentPlayer, laneId, card);
        }
    }
}
