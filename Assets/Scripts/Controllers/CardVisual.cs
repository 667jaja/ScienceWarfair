using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class CardVisual : MonoBehaviour, IPointerClickHandler
{
    //[SerializeField] private SpriteRenderer cardArtUI;
    private Animator anim;
    private Collider2D col;
    private Vector3 startDragPos;
    [SerializeField] private List<Image> cardArtUI = new List<Image>();
    [SerializeField] private List<TMP_Text> titleUI, placementCostUI, iqUI, healthUI, descriptionUI = new List<TMP_Text>();
    [SerializeField] private List<GameObject> actionDisable = new List<GameObject>(); // objects to disable if this is an action card
    private Card card;

    void Start()
    {
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }
    void OnMouseDown()
    {
        startDragPos = transform.position;
        transform.position = MousePos();
        if (LaneManager.instance != null) LaneManager.instance.PlaceActionToggle(card.isAction);
        if (DeckBuilderManager.instance != null) DeckBuilderManager.instance.CardSelectedToggle(true);
    }
    public void OnMouseEnter()
    {
        if (CardManager.instance != null)
        {
            CardManager.instance.lastHoveredCardPos = CardManager.instance.currentHeldCards.IndexOf(this);
            if ( CardManager.instance.lastUnhoveredCardPos == CardManager.instance.currentHeldCards.IndexOf(this)) CardManager.instance.lastUnhoveredCardPos = -1;
            CardManager.instance.PositionHeldCards();
        }
    }
    void OnMouseExit()
    {
        if (CardManager.instance != null)
        {
            CardManager.instance.lastUnhoveredCardPos = CardManager.instance.currentHeldCards.IndexOf(this);
            CardManager.instance.PositionHeldCards();
        }
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
        if (LaneManager.instance != null) LaneManager.instance.PlaceActionToggle(false);
        if (DeckBuilderManager.instance != null) DeckBuilderManager.instance.CardSelectedToggle(false);
    }
    Vector2 MousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    public void Initiate(Card newCard)
    {
        card = newCard;

        foreach (Image item in cardArtUI)
        {
            item.sprite = newCard.cardArt;
        }
        foreach (TMP_Text item in titleUI)
        {
            item.text = newCard.title;
        }
        foreach (TMP_Text item in descriptionUI)
        {
            item.text = newCard.description;
        }
        foreach (TMP_Text item in placementCostUI)
        {
            item.text = newCard.placementCost.ToString();
        }
        foreach (TMP_Text item in iqUI)
        {
            item.text = newCard.iq.ToString();
        }
        foreach (TMP_Text item in healthUI)
        {
            item.text = newCard.health.ToString();
        }
        if (card.isAction)
        {
            foreach (GameObject item in actionDisable)
            {
                item.SetActive(false);
            }
        }
    }
    public void OnPointerClick(PointerEventData eventdata)
    {

    }
    public void UpdateVisuals()
    {
        foreach (Image item in cardArtUI)
        {
            item.sprite = card.cardArt;
        }
        foreach (TMP_Text item in titleUI)
        {
            item.text = card.title;
        }
        foreach (TMP_Text item in descriptionUI)
        {
            item.text = card.description;
        }
        foreach (TMP_Text item in placementCostUI)
        {
            item.text = card.placementCost.ToString();
        }
        foreach (TMP_Text item in iqUI)
        {
            item.text = card.iq.ToString();
        }
        foreach (TMP_Text item in healthUI)
        {
            item.text = card.health.ToString();
        }
    }
}
