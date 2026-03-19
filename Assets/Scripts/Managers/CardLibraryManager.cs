using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class CardLibraryManager : MonoBehaviour
{
    public static CardLibraryManager instance;
    public List<CardData> cardDatas;
    public List<Effect> effects = new List<Effect>();
    public List<EffectTriggerData> EffectTriggerDatas;
    public List<CardTag> cardTags;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        List<string> duplicateNames = new();
        int i;
        int j;
        for (i = 0; i < cardDatas.Count; i++)
        {
            for (j = i+1; j < cardDatas.Count; j++)
            {
                if (cardDatas[i].CardDataId == cardDatas[j].CardDataId)
                {
                    duplicateNames.Add(cardDatas[i].name);
                    break;
                }
            }
        }
        for (i = 0; i < effects.Count; i++)
        {
            for (j = i+1; j < effects.Count; j++)
            {
                if (effects[i].effectId == effects[j].effectId)
                {
                    duplicateNames.Add(effects[i].name);
                    break;
                }
            }
        }
        for (i = 0; i < EffectTriggerDatas.Count; i++)
        {
            for (j = i+1; j < EffectTriggerDatas.Count; j++)
            {
                if (EffectTriggerDatas[i].effectTriggerId == EffectTriggerDatas[j].effectTriggerId)
                {
                    duplicateNames.Add(EffectTriggerDatas[i].name);
                    break;
                }
            }
        }
        for (i = 0; i < cardTags.Count; i++)
        {
            for (j = i+1; j < cardTags.Count; j++)
            {
                if (cardTags[i].tagId == cardTags[j].tagId)
                {
                    duplicateNames.Add(cardTags[i].name);
                    break;
                }
            }
        }
        foreach (string name in duplicateNames)
        {
            Debug.Log("DUPLICATE FOUND: " + name);
        }

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
    public CardTag TagById(int cardTagId)
    {
        CardTag foundCardTag = null;

        foreach(CardTag cardTag in cardTags)
        {
            if (cardTag.tagId == cardTagId)
            {
                foundCardTag = cardTag;
                break;
            }
        }
        return foundCardTag;
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
    public EffectTrigger EffectTriggerFromEffectTriggerStruct(EffectTriggerStruct effectTriggerStruct, Card OriginCard = null)
    {
        EffectTrigger newEffectTrigger = new EffectTrigger(new ActionData(effectTriggerStruct.originPlayer, new Vector2Int(-1,-1), OriginCard), GetEffectTriggerDataById(effectTriggerStruct.effectTriggerId))
        {
            countDownVal = effectTriggerStruct.countDownVal,
            originPlayer = effectTriggerStruct.originPlayer,
            originUnitInstanceId = effectTriggerStruct.originUnitInstanceId,
            savedCardInstanceId = effectTriggerStruct.savedCardInstanceId,
            triggerDisabled = effectTriggerStruct.triggerDisabled,
            effects = new List<Effect>(),
        };

        if (effectTriggerStruct.effects != null && effectTriggerStruct.effects.Length > 0)
        for (int i = 0; i < effectTriggerStruct.effects.Length; i++)
        {
            if (effectTriggerStruct.effects[i] > 0) newEffectTrigger.effects.Add(GetEffectById(effectTriggerStruct.effects[i]));
        }

        return newEffectTrigger;
    }
    public Card CardFromCardStruct(CardStruct cardStruct)
    {
        Card newCard = new Card(GetCardDataById(cardStruct.CardDataBaseId))
        {
            cardInstanceId = cardStruct.cardInstanceId,
            Iq = cardStruct.iq,
            PlacementCost = cardStruct.placementCost,
            Health = cardStruct.health,

            containedCard = cardStruct.containedCardBaseId>=0? GetCardDataById(cardStruct.containedCardBaseId): null,

            tags = new List<CardTag>(),
            effectTriggers = new List<EffectTrigger>(),
        };
        
        if (cardStruct.tagIds != null && cardStruct.tagIds.Length > 0)
        for (int i = 0; i < cardStruct.tagIds.Length; i++)
        {
            CardTag newCardTag = TagById(cardStruct.tagIds[i]);
            newCard.tags.Add(newCardTag);
        }
        
        if (cardStruct.effectTriggers != null && cardStruct.effectTriggers.Length > 0)
        for (int i = 0; i < cardStruct.effectTriggers.Length; i++)
        {
            EffectTrigger newEffectTrigger = EffectTriggerFromEffectTriggerStruct(cardStruct.effectTriggers[i]);
            newCard.effectTriggers.Add(newEffectTrigger);
        }
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
        newPlayer.rawDeck = new();
        newPlayer.opener = new();
        newPlayer.deck = new();
        newPlayer.hand = new();
        
        //rawDeck
        for (int i = 0; i < playerStruct.rawDeck.Length; i++)
        {
            newPlayer.rawDeck.Add(GetCardDataById(playerStruct.rawDeck[i]));
        }

        //opener
        for (int i = 0; i < playerStruct.opener.Length; i++)
        {
            newPlayer.opener.Add(GetCardDataById(playerStruct.opener[i]));
        }

        //deck
        for (int i = 0; i < playerStruct.deck.Length; i++)
        {
            newPlayer.deck.Add(GetCardDataById(playerStruct.deck[i]));
        }

        //hand
        for (int i = 0; i < playerStruct.hand.Length; i++)
        {
            newPlayer.hand.Add(CardFromCardStruct(playerStruct.hand[i]));
        }

        //units
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (playerStruct.units[(i*3) + j].CardDataBaseId > 0) newPlayer.units[i, j] = CardFromCardStruct(playerStruct.units[(i*3) + j]);
                else newPlayer.units[i, j] = null;
            }        
        }

        return newPlayer;
    }
}
