using UnityEngine;

public class CardDropArea : MonoBehaviour, ICardDropArea
{
    public void OnCardDrop(CardVisual card)
    {
        card.transform.parent = this.transform;
        card.transform.position = Vector3.zero;
    }
}
