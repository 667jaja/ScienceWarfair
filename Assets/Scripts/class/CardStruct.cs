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
    public EffectTriggerStruct[] effectTriggers;
    public int[] effects;
    public int containedCardBaseId;

    public CardStruct(Card card)
    {
        CardDataBaseId = card.cardData.CardDataId;
        cardInstanceId = card.cardInstanceId;
        placementCost = card.PlacementCost;
        iq = card.Iq;
        health = card.Health;

        containedCardBaseId = card.containedCard != null? card.containedCard.CardDataId: -1;
        effectTriggers = new EffectTriggerStruct[card.effectTriggers.Count];
        effects = new int[card.effects.Count];


        if (card.effectTriggers == null || card.effectTriggers.Count < 1)
        {
            effectTriggers = new EffectTriggerStruct[0];
        }
        else for (int i = 0; i < card.effectTriggers.Count; i++)
        {
            if (card.effectTriggers[i] != null) effectTriggers[i] = new EffectTriggerStruct(card.effectTriggers[i]);
        }

        if (card.effects == null || card.effects.Count < 1)
        {
            effects = new int[1] {0};
        }
        else for (int i = 0; i < card.effects.Count; i++)
        {
            if (card.effects[i] != null) effects[i] = card.effects[i].effectId;
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

        // effectTriggers
        int triggersLength = effectTriggers != null ? effectTriggers.Length : 0;
        serializer.SerializeValue(ref triggersLength);

        if (serializer.IsReader)
            effectTriggers = new EffectTriggerStruct[triggersLength];

        for (int i = 0; i < triggersLength; i++)
            serializer.SerializeValue(ref effectTriggers[i]);

        // effects
        int effectsLength = effects != null ? effects.Length : 0;
        serializer.SerializeValue(ref effectsLength);

        if (serializer.IsReader)
            effects = new int[effectsLength];

        for (int i = 0; i < effectsLength; i++)
            serializer.SerializeValue(ref effects[i]);
    }
};
