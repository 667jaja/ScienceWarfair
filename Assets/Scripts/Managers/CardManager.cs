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
    [SerializeField] private Transform cardCanvas;
    [SerializeField] private Transform playerHand;
    [SerializeField] private TMP_Text deckCountUI;
    private List<Card> deck = new List<Card>();
    private int deckSize = 10;

    void Awake()
    {
        instance = this;
    }
    public void DrawCard()
    {
        Debug.Log("card Draw Called");
        if (ActionManager.instance.isPerforming) return;

        DrawCardGA drawCardGA = new();
        ActionManager.instance.Perform(drawCardGA);
    }

    private void OnEnable()
    {
        ActionManager.AttachPerformer<DrawCardGA>(DrawCardPerformer);
    }
    private void OnDisable()
    {
        ActionManager.DetachPerformer<DrawCardGA>();
    }
    private IEnumerator DrawCardPerformer(DrawCardGA drawCardGA)
    {
        Debug.Log("card Draw Performed");

        //CardVisual card = Instantiate(cardVisual, playerHand.position, Quaternion.identity);
        if (deck.Count < 1)
        {
            ShuffleDiscardIntoDeck();
        }
        Card drawnCard = deck[0];
        deck.Remove(drawnCard);
        CardVisual newCardVisual = Instantiate(cardVisual, playerHand);
        newCardVisual.Initiate(drawnCard);
        UpdateDeckUI();
        yield return null;
    }
    public void CreateDeck()
    {
        deck = new();
        for (int i = 0; i < deckSize; i++)
        {
            CardData data = cardDatas[Random.Range(0, cardDatas.Count)];
            Card newCard = new(data);
            deck.Add(newCard);
        }
        UpdateDeckUI();
    }
    public void ShuffleDiscardIntoDeck()
    {
        CreateDeck();
        UpdateDeckUI();
    }
    public void UpdateDeckUI()
    {
        deckCountUI.text = deck.Count.ToString();
    }
}
