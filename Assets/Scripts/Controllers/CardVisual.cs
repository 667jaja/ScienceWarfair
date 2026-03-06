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
    private Card card;
    
    private Vector3 startDragPos;
    [SerializeField] private bool isUnit;

    //UI
    [SerializeField] private List<Image> cardArtUI = new List<Image>();
    [SerializeField] private List<TMP_Text> titleUI, placementCostUI, iqUI, healthUI, descriptionUI = new List<TMP_Text>();
    [SerializeField] private List<GameObject> actionDisable = new List<GameObject>(); // objects to disable if this is an action card

    //effectInfos
    [SerializeField] private Transform effectInfosSpawn;
    [SerializeField] private float effectInfoYAdd;
    [SerializeField] private GameObject effectTriggerInfoPrefab;
    [SerializeField] private GameObject containedCardInfoPrefab;
    private List<GameObject> effectInfos = new List<GameObject>();

    void Start()
    {
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }
    void OnMouseDown()
    { 
        if (isUnit) return;
        
        startDragPos = transform.position;
        transform.position = MousePos();
        if (LaneManager.instance != null) LaneManager.instance.PlaceActionToggle(card.isAction);
        if (DeckBuilderManager.instance != null) DeckBuilderManager.instance.CardSelectedToggle(true);
    }
    public void OnMouseEnter()
    {
        if (isUnit) return;
        if (CardManager.instance != null)
        {
            CardManager.instance.lastHoveredCardPos = CardManager.instance.currentHeldCards.IndexOf(this);
            if ( CardManager.instance.lastUnhoveredCardPos == CardManager.instance.currentHeldCards.IndexOf(this)) CardManager.instance.lastUnhoveredCardPos = -1;
            CardManager.instance.PositionHeldCards();
        }
    }
    void OnMouseExit()
    {
        if (isUnit) return;
        if (CardManager.instance != null)
        {
            CardManager.instance.lastUnhoveredCardPos = CardManager.instance.currentHeldCards.IndexOf(this);
            CardManager.instance.PositionHeldCards();
        }
    }
    void OnMouseDrag()
    {
        if (isUnit) return;
        transform.position = MousePos();
    }
    void OnMouseUp()
    {
        if (isUnit) return;
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
            item.text = newCard.PlacementCost.ToString();
        }
        foreach (TMP_Text item in iqUI)
        {
            item.text = newCard.Iq.ToString();
        }
        foreach (TMP_Text item in healthUI)
        {
            item.text = newCard.Health.ToString();
        }
        if (card.isAction)
        {
            foreach (GameObject item in actionDisable)
            {
                item.SetActive(false);
            }
        }
        if (isUnit)
        {
            UpdateEffectInfos();
        }
    }
    public void OnPointerClick(PointerEventData eventdata)
    {

    }
    public void UnitSelectionButton()
    {
        UnitManager.instance.UnitSelected(this);
    }
    public void CardSelectionButton()
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
            item.text = card.PlacementCost.ToString();
        }
        foreach (TMP_Text item in iqUI)
        {
            item.text = card.Iq.ToString();
        }
        foreach (TMP_Text item in healthUI)
        {
            item.text = card.Health.ToString();
        }
        UpdateEffectInfos();
    }
    private void UpdateEffectInfos()
    {
        foreach (GameObject item in effectInfos)
        {
            Destroy(item);
        }
        effectInfos = new();
        //Effect Triggers (bottom)
        foreach (EffectTrigger ET in card.effectTriggers)
        {
            GameObject newLoggedAction = Instantiate(effectTriggerInfoPrefab, effectInfosSpawn);

            foreach (GameObject item in effectInfos)
            {
                item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y + effectInfoYAdd, item.transform.position.z);
            }

            if (newLoggedAction.GetComponent<EffectInfoVisual>() != null) newLoggedAction.GetComponent<EffectInfoVisual>().InitiateET(ET.GetTriggerName(), ET.GetEffectName(), ET.countDownVal, (ET.countDownVal <= 0 && ET.countDown <= 0), ET.triggerDisabled);

            effectInfos.Add(newLoggedAction);
        }
        //Contained Cards (top)
        if (card.containedCard != null)
        {
            GameObject newLoggedAction = Instantiate(containedCardInfoPrefab, effectInfosSpawn);

            foreach (GameObject item in effectInfos)
            {
                item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y + effectInfoYAdd, item.transform.position.z);
            }
            
            if (newLoggedAction.GetComponent<EffectInfoVisual>() != null) newLoggedAction.GetComponent<EffectInfoVisual>().InitiateCC(card.containedCard.title, card.containedCard.health, card.containedCard.placementCost, card.containedCard.iq, card.containedCard.description);

            effectInfos.Add(newLoggedAction);
        }
    }
}
