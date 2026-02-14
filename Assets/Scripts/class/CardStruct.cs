using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
public struct CardStruct : INetworkSerializable
{
    //card data
    public int CardDataBaseId;

    //stats
    public int placementCost;
    public int iq;
    public int health;

    //abilities
    public int[] effectTriggers;
    public int[] effects;
    public int containedCardBaseId;
    private bool hasBeenSet;

    public CardStruct(Card card)
    {
        hasBeenSet = true;

        CardDataBaseId = card.cardData.CardDataId;
        placementCost = card.placementCost;
        iq = card.iq;
        health = card.health;

        containedCardBaseId = card.containedCard.CardDataId;
        effectTriggers = new int[card.effectTriggers.Count];
        effects = new int[card.effects.Count];


        if (card.effectTriggers == null || card.effectTriggers.Count < 1)
        {
            effects = new int[1] {0};
        }
        else for (int i = 0; i < card.effectTriggers.Count; i++)
        {
            if (card.effectTriggers[i] != null) effectTriggers[i] = card.effectTriggers[i].effectTriggerData.effectTriggerId;
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
        if (hasBeenSet != true) return;

        serializer.SerializeValue(ref CardDataBaseId);

        serializer.SerializeValue(ref placementCost);
        serializer.SerializeValue(ref iq);
        serializer.SerializeValue(ref health);

        serializer.SerializeValue(ref containedCardBaseId);


        //ARRAYS

        //ARRAYS 

        //serialize length

        int array1Length = effectTriggers != null ? effectTriggers.Length : 0;
        int array2Length = effects != null ? effects.Length : 0;


        serializer.SerializeValue(ref array1Length);
        serializer.SerializeValue(ref array2Length);

        //allocate array
        if (serializer.IsReader)
        {
            effectTriggers = new int[array1Length];
            effects = new int[array2Length];
        }

        // serialize elements
        for (int i = 0; i < array1Length; i++)
        {
            serializer.SerializeValue(ref effectTriggers[i]);
        }

        for (int i = 0; i < array2Length; i++)
        {
            serializer.SerializeValue(ref effects[i]);
        }
    }

};
