using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class CardLibraryManager : MonoBehaviour
{
    public static CardLibraryManager instance;
    public List<CardData> cardDatas;
    public List<Effect> effects = new List<Effect>();
    public List<EffectTriggerData> EffectTriggerDatas;
    void Awake()
    {
        instance = this;
    }
    public CardData GetCardDataById(int cardId)
    {
        CardData foundCard = null;

        foreach(CardData card in cardDatas)
        {
            if (card.CardDataId == cardId)
            {
                foundCard = card;
                break;
            }
        }
        return foundCard;
    }
    public Effect GetEffectById(int effectId)
    {
        Effect foundEffect = null;

        foreach(Effect effect in effects)
        {
            if (effect.effectId == effectId)
            {
                foundEffect = effect;
                break;
            }
        }
        return foundEffect;
    }
    public EffectTriggerData GetEffectTriggerDataById(int EffectTriggerId)
    {
        EffectTriggerData foundEffectTrigger = null;

        foreach(EffectTriggerData et in EffectTriggerDatas)
        {
            if (et.effectTriggerId == EffectTriggerId)
            {
                foundEffectTrigger = et;
                break;
            }
        }
        return foundEffectTrigger;
    }

    public Card cardFromCardStruct(CardStruct cardStruct)
    {
        Card newCard = new Card(GetCardDataById(cardStruct.CardDataBaseId))
        {
            iq = cardStruct.iq,
            placementCost = cardStruct.placementCost,
            health = cardStruct.health,

            containedCard = cardStruct.containedCardBaseId>=0? GetCardDataById(cardStruct.containedCardBaseId): null,

            // effectTriggers = cardStruct.health,
            effects = new List<Effect>(),
        };
        if (cardStruct.effects != null && cardStruct.effects.Length > 0)
        for (int i = 0; i < cardStruct.effects.Length; i++)
        {
            if (cardStruct.effects[i] > 0) newCard.effects.Add(GetEffectById(cardStruct.effects[i]));
        }
        
        // if (cardStruct.effectTriggers != null && cardStruct.effectTriggers.Length > 0)
        // for (int i = 0; i < cardStruct.effectTriggers.Length; i++)
        // {
        //     if (cardStruct.effectTriggers[i] > 0) newCard.effectTriggers.Add(new EffectTrigger(new ActionData(,,newCard),GetEffectTriggerDataById(cardStruct.effectTriggers[i])));
        // }
        return newCard;
    }
    public Player PlayerFromPlayerStruct(PlayerStruct playerStruct)
    {
        //if (playerStruct.name == null) Debug.Log("playerNull");
        Player newPlayer = new Player(null, playerStruct.id)
        {
            name = new string(playerStruct.name),
            maxMoney = playerStruct.maxMoney,
            id = playerStruct.id,
            sciencePoints = playerStruct.sciencePoints,
            Money = playerStruct.money,
            actionPoints = playerStruct.actionPoints
        };
        newPlayer.deck = new();
        newPlayer.rawDeck = new();
        newPlayer.hand = new();

        //deck
        for (int i = 0; i < playerStruct.deck.Length; i++)
        {
            newPlayer.deck.Add(GetCardDataById(playerStruct.deck[i]));
        }

        //rawDeck
        for (int i = 0; i < playerStruct.rawDeck.Length; i++)
        {
            newPlayer.rawDeck.Add(GetCardDataById(playerStruct.rawDeck[i]));
        }

        //hand
        for (int i = 0; i < playerStruct.hand.Length; i++)
        {
            newPlayer.hand.Add(cardFromCardStruct(playerStruct.hand[i]));
        }

        //units
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (playerStruct.units[(i*3) + j].CardDataBaseId > 0) newPlayer.units[i, j] = cardFromCardStruct(playerStruct.units[(i*3) + j]);
                else newPlayer.units[i, j] = null;
            }        
        }

        return newPlayer;
    }
}
