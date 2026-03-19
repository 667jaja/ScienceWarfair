using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.Collections;

// ISSUE: online players keep losing cards (maybe specifically draw card actions) when they draw them
public class CardManager : MonoBehaviour
{
    // manages backend actions of cards while is deck, discard
    // controls frontend actions of deck and discard
    public static CardManager instance;

    [SerializeField] private CardVisual cardVisual;
    public List<CardVisual> currentHeldCards = new List<CardVisual>();

    [SerializeField] private Transform cardCanvas;
    [SerializeField] private Transform playerHand;
    [SerializeField] private Transform deck;
    [SerializeField] private TMP_Text deckCountUI;
    [SerializeField] private GameObject cardFlipPrefab; //placeholder draw anim
    [SerializeField] private GameObject cardFlip; //placeholder draw anim



    //handWidthVar
    [SerializeField] private float maxHandWidth;
    [SerializeField] private float handWidthScaleRate;
    [SerializeField] private float selectedCardRiseDistance;

    public int lastHoveredCardPos;
    public int lastUnhoveredCardPos = -1;
    private bool cardIsHovered;
    private int deckSize = 10;

    void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        ActionManager.AttachPerformer<DrawCardGA>(DrawCardPerformer);
        ActionManager.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionManager.AttachPerformer<DiscardCardGA>(DiscardCardPerformer);
        ActionManager.AttachPerformer<DiscardCardsGA>(DiscardCardsPerformer);
        ActionManager.AttachPerformer<DiscardSelectedGA>(DiscardSelectedPerformer);
    }
    private void OnDisable()
    {
        ActionManager.DetachPerformer<DrawCardGA>();
        ActionManager.DetachPerformer<DrawCardsGA>();
        ActionManager.DetachPerformer<DiscardCardGA>();
        ActionManager.DetachPerformer<DiscardCardsGA>();
        ActionManager.DetachPerformer<DiscardSelectedGA>();
    }
    public void DrawCards(int playerId, int count = 1)
    {
        Debug.Log("card Draws");

        for (int i = 0; i < count; i++)
        {
            DrawCardGA drawCardGA = new(playerId);
            ActionManager.instance.Perform(drawCardGA);
        }
    }
    public void CurrentPlayerDrawCards(int count = 1)
    {
        Debug.Log("Current Player card Draw");

        DrawCards(GameManager.instance.currentPlayer, count);
    }
    private IEnumerator DrawCardsPerformer(DrawCardsGA drawCardsGA)
    {
        //Debug.Log("Multi card Draw");

        for (int i = 0; i < drawCardsGA.drawCount; i++)
        {
            DrawCardGA drawCardGA = new(drawCardsGA.playerId);
            //drawCardGA.description = GameManager.instance.players[drawCardsGA.playerId].name + " draws a card";
            ActionManager.instance.AddReaction(drawCardGA);
        }
        yield return null;
    }
    private IEnumerator DrawCardPerformer(DrawCardGA drawCardGA)
    {
        Debug.Log("card Draw Started");

        //
        if (drawCardGA.playerId == GameManager.instance.displayPlayer)
        {
            if (cardFlip == null)
            {
                cardFlip = Instantiate(cardFlipPrefab, cardCanvas);
            }
            cardFlip.SetActive(true);
            cardFlip.transform.position = deck.position;
            cardFlip.GetComponent<Animator>().SetTrigger("flip");
            float moveRate = 4;
            float timer = 0;
            //move 
            while (timer < 1/moveRate)
            {
                timer += Time.deltaTime;
                cardFlip.transform.position = Vector3.Lerp(deck.position, playerHand.position, moveRate*timer);
                yield return null;
            }
            cardFlip.SetActive(false);
        }

        //backend
        if (GameManager.instance.players[drawCardGA.playerId].deck.Count < 1)
        {
            ShuffleDiscardIntoDeck(drawCardGA.playerId);
        }
        
        AddtoHand(drawCardGA.playerId,  GameManager.instance.players[drawCardGA.playerId].deck[0]);
        GameManager.instance.players[drawCardGA.playerId].deck.RemoveAt(0);

        //frontend
        UpdateHandUI();
        UpdateDeckUI();

        // if (drawCardGA.playerId == GameManager.instance.)
        // {
        //     CardVisual newCardVisual = Instantiate(cardVisual, playerHand);
        //     newCardVisual.Initiate(drawnCard);
        //     currentHeldCards.Add(newCardVisual);
        // }
        // else
        // {
        //     Debug.Log("other player draws card");
        // }

        Debug.Log("Draw Card Ended");
        yield return null;
    }
    public void AddtoHand(int playerId, CardData carddata)
    {
        Card drawnCard = new(carddata);
        GameManager.instance.players[playerId].hand.Add(drawnCard);
    }
    public List<CardData> CreateDeck(List<CardData> deckData)
    {
        return deckData.OrderBy(i => Guid.NewGuid()).ToList();
    }
    public void ShuffleDiscardIntoDeck(int playerId)
    {
        GameManager.instance.players[playerId].deck = CreateDeck(GameManager.instance.players[playerId].rawDeck);
        UpdateDeckUI();
    }
    private IEnumerator DiscardSelectedPerformer(DiscardSelectedGA discardSelectedGA)
    {
        DiscardCardsGA discardCardsGA =  new DiscardCardsGA(discardSelectedGA.playerId, SelectionManager.instance.selectedHand);
        ActionManager.instance.AddReaction(discardCardsGA);

        yield return null;
    }
    private IEnumerator DiscardCardsPerformer(DiscardCardsGA discardCardsGA)
    {
        for (int i = 0; i < discardCardsGA.discardedCards.Count; i++)
        {
            DiscardCardGA discardCardGA = new(discardCardsGA.playerId, discardCardsGA.discardedCards[i]);
            //drawCardGA.description = GameManager.instance.players[drawCardsGA.playerId].name + " draws a card";
            ActionManager.instance.AddReaction(discardCardGA);
        }
        yield return null;
    }
    private IEnumerator DiscardCardPerformer(DiscardCardGA discardCardGA)
    {
        RemoveCard(discardCardGA.playerId, discardCardGA.discardedCard);
        UpdateHandUI();
        yield return null;
    }
    public bool RemoveCard(int playerId, Card removedCard)
    {
        //find card in hand

        int cardToRemove = -1;
        bool cardRemoved = false;
        int i = 0;
        foreach (Card card in GameManager.instance.players[playerId].hand)
        {
            if (card.cardInstanceId == removedCard.cardInstanceId)
            {
                //if (card.health == removedCard.health && card.iq == removedCard.iq && card.placementCost == removedCard.placementCost)
                cardToRemove = i;
                cardRemoved = true;
                break;
            }
            i++;
        }
        if (!cardRemoved)
        {
            i=0;
            foreach (Card card in GameManager.instance.players[playerId].hand)
            {
                if (card.cardData.CardDataId == removedCard.cardData.CardDataId)
                {
                    //if (card.health == removedCard.health && card.iq == removedCard.iq && card.placementCost == removedCard.placementCost)
                    cardRemoved = true;
                    cardToRemove = i;
                    break;
                }
                i++;
            }
            if (cardRemoved)
            {
                Debug.Log("Card not found by Id, removed identical Card");
            }
            else
            {
                Debug.Log("removed Card not found");
            }
        }


        //remove card
        bool cardRemovalSuccessful;
        if (cardToRemove >= 0)
        {
            GameManager.instance.players[playerId].hand.RemoveAt(cardToRemove);
            cardRemovalSuccessful = true;
        }
        else
        {
            Debug.Log("Removed card not found");
            cardRemovalSuccessful = false;
        }
        
        return cardRemovalSuccessful;
    }
    public Card FindCardInHand(int playerId, int cardInstanceId, int CardDataId)
    {
        //find card in hand

        bool cardFound = false;
        Card foundCard = null;
        int i = 0;
        foreach (Card card in GameManager.instance.players[playerId].hand)
        {
            if (card.cardInstanceId == cardInstanceId)
            {
                foundCard = card;
                cardFound = true;
                break;
            }
            i++;
        }
        if (!cardFound)
        {
            i=0;
            foreach (Card card in GameManager.instance.players[playerId].hand)
            {
                if (card.cardData.CardDataId == CardDataId)
                {
                    foundCard = card;
                    cardFound = true;
                    break;
                }
                i++;
            }
            if (cardFound)
            {
                Debug.Log("Card not found by Id, returned identical Card");
            }
            else
            {
                Debug.Log("Card not found");
            }
        }
        return foundCard;
    }
    public CardVisual FindCardVisualInHand(int cardInstanceId, int CardDataId)
    {
        //find card in hand

        bool cardFound = false;
        CardVisual foundCard = null;
        int i = 0;
        foreach (CardVisual cardVisual in currentHeldCards)
        {
            if (cardVisual.card.cardInstanceId == cardInstanceId)
            {
                foundCard = cardVisual;
                cardFound = true;
                break;
            }
            i++;
        }
        if (!cardFound)
        {
            i=0;
            foreach (CardVisual card in currentHeldCards)
            {
                if (cardVisual.card.cardData.CardDataId == CardDataId)
                {
                    foundCard = card;
                    cardFound = true;
                    break;
                }
                i++;
            }
            if (cardFound)
            {
                Debug.Log("Card not found by Id, returned identical Card");
            }
            else
            {
                Debug.Log("Card not found");
            }
        }
        return foundCard;
    }
    public void MakeHandSelectable(Card card)
    {
        CardVisual cardInHand = FindCardVisualInHand(card.cardInstanceId, card.cardData.CardDataId);
        cardInHand.SetSelectable(true);
    }
    public void MakeHandUnselectable()
    {
        foreach(CardVisual card in currentHeldCards)
        {
            card.SetSelectable(false);
        }
    }
    public void HandSelected(CardVisual visual)
    {
        SelectionManager.instance.selectedHand.Add(visual.card);
    }
    public void UpdateDeckUI()
    {
        deckCountUI.text = GameManager.instance.players[GameManager.instance.displayPlayer].deck.Count.ToString();
    }
    public void UpdateHandUI()
    {
        foreach (CardVisual item in currentHeldCards)
        {
            Destroy(item.gameObject);
        }
        currentHeldCards = new();
        foreach (Card item in GameManager.instance.players[GameManager.instance.displayPlayer].hand)
        {
            CardVisual newCardVisual = Instantiate(cardVisual, playerHand);
            newCardVisual.Initiate(item);
            currentHeldCards.Add(newCardVisual);
        }
        PositionHeldCards();
    }
    public void PositionHeldCards()
    {
        int i = -1;
        int cardsHeld = currentHeldCards.Count;
        float handWidth = -(maxHandWidth * handWidthScaleRate) / (cardsHeld + handWidthScaleRate) + maxHandWidth;

        cardIsHovered = !(lastUnhoveredCardPos == lastHoveredCardPos);

        if (lastHoveredCardPos > cardsHeld-1 || lastHoveredCardPos < 0 || currentHeldCards[lastHoveredCardPos] == null)
        {
            cardIsHovered = false;
            lastHoveredCardPos = cardsHeld-1;
        }

        foreach (CardVisual item in currentHeldCards)
        {
            item.transform.position = new Vector3(playerHand.position.x-handWidth/2 + (handWidth/(cardsHeld+1))*(i+1), playerHand.position.y);//(cardIsHovered && i<lastHoveredCardPos)? -i*2 - cardsHeld*2: -i*2);
            i++;
        }

        //lift Selected
        int j = 2;
        if (lastHoveredCardPos > 0)
        {
            for (i = lastHoveredCardPos-1; i >= 0; i--)
            {
                currentHeldCards[i].transform.SetAsFirstSibling();
                currentHeldCards[i].transform.position = new Vector3(currentHeldCards[i].transform.position.x, currentHeldCards[i].transform.position.y, j);
                j+=2;
            }
        }
        if (lastHoveredCardPos < cardsHeld-1)
        {
            for (i = lastHoveredCardPos+1; i < cardsHeld; i++)
            {
                currentHeldCards[i].transform.SetAsFirstSibling();
                currentHeldCards[i].transform.position = new Vector3(currentHeldCards[i].transform.position.x, currentHeldCards[i].transform.position.y, j);
                j+=2;
            }
        }


        if (cardIsHovered)
        {
            Vector3 orig = currentHeldCards[lastHoveredCardPos].transform.position;
            currentHeldCards[lastHoveredCardPos].transform.position = new Vector3(orig.x, orig.y + selectedCardRiseDistance, orig.z);
        }
    }
}
