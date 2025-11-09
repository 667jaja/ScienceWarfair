using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour, IPointerClickHandler
{
    //[SerializeField] private SpriteRenderer cardArtUI;
    Collider2D col;
    private Vector3 startDragPos;
    [SerializeField] private Image cardArtUI;
    [SerializeField] private TMP_Text titleUI;
    [SerializeField] private TMP_Text placementCostUI;
    private Card card;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }
    void OnMouseDown()
    {
        startDragPos = transform.position;
        transform.position = MousePos();
    }
    void OnMouseDrag()
    {
        transform.position = MousePos();
    }
    void OnMouseUp()
    {
        // DropCard();
        col.enabled = false;
        Collider2D hitCollider = Physics2D.OverlapPoint(MousePos());
        col.enabled = true;
        if (hitCollider != null && hitCollider.TryGetComponent(out ICardDropArea cardDropArea))
        {
            cardDropArea.OnCardDrop(card);
        }
        else
        {
            transform.position = startDragPos;
        }
    }
    Vector2 MousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    public void Initiate(Card newCard)
    {
        card = newCard;
        cardArtUI.sprite = newCard.cardArt;
        titleUI.text = newCard.title;
        if (placementCostUI != null) placementCostUI.text = newCard.placementCost.ToString();
    }

    public void OnPointerClick(PointerEventData eventdata)
    {

    }
    // public void DropCard()
    // {
    //     card.PerformEffect();
    //     Destroy(gameObject);
    // }
}
