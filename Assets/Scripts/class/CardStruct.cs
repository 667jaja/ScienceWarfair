using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
public struct CardStruct : INetworkSerializable
{
    //card data
    public int CardDataBaseId;
    public int cardInstanceId;

    //stats
    public int placementCost;
    public int iq;
    public int health;

    //abilities
    public int[] tagIds;
    public EffectTriggerStruct[] effectTriggers;
    public int containedCardBaseId;

    public CardStruct(Card card)
    {
        CardDataBaseId = card.cardData.CardDataId;
        cardInstanceId = card.cardInstanceId;
        placementCost = card.PlacementCost;
        iq = card.Iq;
        health = card.Health;

        containedCardBaseId = card.containedCard != null? card.containedCard.CardDataId: -1;
        
        int i;

        tagIds = new int[card.tags.Count];
        if (card.tags != null && card.tags.Count > 0) for (i = 0; i < card.tags.Count; i++)
        {
            if (card.tags[i] != null) tagIds[i] = card.tags[i].tagId;
        }

        effectTriggers = new EffectTriggerStruct[card.effectTriggers.Count];
        if (card.effectTriggers != null && card.effectTriggers.Count > 0) for (i = 0; i < card.effectTriggers.Count; i++)
        {
            if (card.effectTriggers[i] != null) effectTriggers[i] = new EffectTriggerStruct(card.effectTriggers[i]);
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref CardDataBaseId);
        serializer.SerializeValue(ref cardInstanceId);
        serializer.SerializeValue(ref placementCost);
        serializer.SerializeValue(ref iq);
        serializer.SerializeValue(ref health);
        serializer.SerializeValue(ref containedCardBaseId);

        // tags
        int tagsLength = tagIds != null ? tagIds.Length : 0;
        serializer.SerializeValue(ref tagsLength);

        if (serializer.IsReader)
            tagIds = new int[tagsLength];

        for (int i = 0; i < tagsLength; i++)
            serializer.SerializeValue(ref tagIds[i]);

        // effectTriggers
        int triggersLength = effectTriggers != null ? effectTriggers.Length : 0;
        serializer.SerializeValue(ref triggersLength);

        if (serializer.IsReader)
            effectTriggers = new EffectTriggerStruct[triggersLength];

        for (int i = 0; i < triggersLength; i++)
            serializer.SerializeValue(ref effectTriggers[i]);
    }
};
