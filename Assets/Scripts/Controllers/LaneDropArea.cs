using UnityEngine;

public class LaneDropArea : MonoBehaviour, ICardDropArea
{
    [SerializeField] private int laneId;
    public void OnCardDrop(Card card)
    {
        int currentPlayer = GameManager.instance.currentPlayer;
    
        // Debug.Log("player id: " + currentPlayer);
        // Debug.Log("lane id: " + laneId);
        // Debug.Log("card name: " + card.title);
        // card.transform.parent = this.transform;
        // card.transform.position = Vector3.zero;
        if (!UnitManager.instance.TryPlaceCard(currentPlayer, laneId, card))
        {
            GameManager.instance.GlobalUIUpdate();
        }
    }
}
