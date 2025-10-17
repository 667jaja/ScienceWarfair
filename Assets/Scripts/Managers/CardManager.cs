using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class CardManager : MonoBehaviour
{
    // manages backend actions of cards while is deck, discard
    // controls frontend actions of deck and discard
    public static CardManager instance;
    [SerializeField] private List<CardData> cardDatas;
    [SerializeField] private CardVisual cardVisual;
    [SerializeField] private List<CardVisual> currentHeldCards = new List<CardVisual>();
    [SerializeField] private Transform cardCanvas;
    [SerializeField] private Transform playerHand;
    [SerializeField] private TMP_Text deckCountUI;

    //handWidthVar
    [SerializeField] private float maxHandWidth;
    [SerializeField] private float handWidthScaleRate;
    private int deckSize = 10;

    void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        ActionManager.AttachPerformer<DrawCardGA>(DrawCardPerformer);
    }
    private void OnDisable()
    {
        ActionManager.DetachPerformer<DrawCardGA>();
    }
    public void DrawCard(int playerId)
    {
        if (ActionManager.instance.isPerforming) return;

        DrawCardGA drawCardGA = new(playerId);
        ActionManager.instance.Perform(drawCardGA);
    }
    public void CurrentPlayerDrawCard()
    {
        if (ActionManager.instance.isPerforming) return;

        DrawCardGA drawCardGA = new(GameManager.instance.currentPlayer);
        ActionManager.instance.Perform(drawCardGA);
    }
    private IEnumerator DrawCardPerformer(DrawCardGA drawCardGA)
    {
        Debug.Log("card Draw Performed");

        //CardVisual card = Instantiate(cardVisual, playerHand.position, Quaternion.identity);

        //backend
        if (GameManager.instance.players[drawCardGA.playerId].deck.Count < 1)
        {
            ShuffleDiscardIntoDeck(drawCardGA.playerId);
        }
        Card drawnCard = GameManager.instance.players[drawCardGA.playerId].deck[0];
        GameManager.instance.players[drawCardGA.playerId].deck.Remove(drawnCard);
        GameManager.instance.players[drawCardGA.playerId].hand.Add(drawnCard);

        //frontend
        UpdateHandUI();
        // if (drawCardGA.playerId == GameManager.instance.currentPlayer)
        // {
        //     CardVisual newCardVisual = Instantiate(cardVisual, playerHand);
        //     newCardVisual.Initiate(drawnCard);
        //     currentHeldCards.Add(newCardVisual);
        // }
        // else
        // {
        //     Debug.Log("other player draws card");
        // }
        UpdateDeckUI();
        yield return null;
    }
    public void CreateDeck(int playerId)
    {
        GameManager.instance.players[playerId].deck = new();
        for (int i = 0; i < deckSize; i++)
        {
            CardData data = cardDatas[Random.Range(0, cardDatas.Count)];
            Card newCard = new(data);
            GameManager.instance.players[playerId].deck.Add(newCard);
        }
        UpdateDeckUI();
    }
    public void ShuffleDiscardIntoDeck(int playerId)
    {
        CreateDeck(playerId);
        UpdateDeckUI();
    }
    public void UpdateDeckUI()
    {
        deckCountUI.text = GameManager.instance.players[GameManager.instance.currentPlayer].deck.Count.ToString();
    }
    public void UpdateHandUI()
    {
        foreach (CardVisual item in currentHeldCards)
        {
            Destroy(item.gameObject);
        }
        currentHeldCards = new();
        foreach (Card item in GameManager.instance.players[GameManager.instance.currentPlayer].hand)
        {
            CardVisual newCardVisual = Instantiate(cardVisual, playerHand);
            newCardVisual.Initiate(item);
            currentHeldCards.Add(newCardVisual);
        }
        PositionHeldCards();
    }
    private void PositionHeldCards()
    {
        int i = -1;
        float cardsHeld = currentHeldCards.Count;
        float handWidth = -(maxHandWidth * handWidthScaleRate) / (cardsHeld + handWidthScaleRate) + maxHandWidth;
        foreach (CardVisual item in currentHeldCards)
        {
            i++;
            item.transform.position = new Vector2(playerHand.position.x-handWidth/2 + (handWidth/(cardsHeld+1))*(i+1), playerHand.position.y);
        }
    }
}
