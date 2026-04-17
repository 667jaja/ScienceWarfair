using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public struct CardModifier : INetworkSerializable
{

    public string modDescription; 
    //stats
    public int placementCost;
    public int iq;
    public int health;

    //abilities
    public int[] tagIds;
    public int[] effectTriggerDatas; // needs to be replaced with ET instance id or something

    public CardModifier(string modDescription2, int placementCost2, int iq2, int health2, List<int> tagIds2, List<int> effectTriggersDatas2)
    {
        modDescription = modDescription2;
        if (tagIds2 == null) tagIds2 = new();
        if (effectTriggersDatas2 == null) effectTriggersDatas2 = new();

        placementCost = placementCost2;
        iq =  iq2;
        health =  health2;
        
        int i;
        

        tagIds = new int[tagIds2.Count];
        if (tagIds2 != null && tagIds2.Count > 0) for (i = 0; i < tagIds2.Count; i++)
        {
            tagIds[i] = tagIds2[i];
        }

        effectTriggerDatas = new int[effectTriggersDatas2.Count];
        if (effectTriggersDatas2 != null && effectTriggersDatas2.Count > 0) for (i = 0; i < effectTriggersDatas2.Count; i++)
        {
            effectTriggerDatas[i] = effectTriggersDatas2[i];
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref placementCost);
        serializer.SerializeValue(ref iq);
        serializer.SerializeValue(ref health);

        // tags
        int tagsLength = tagIds != null ? tagIds.Length : 0;
        serializer.SerializeValue(ref tagsLength);

        if (serializer.IsReader)
            tagIds = new int[tagsLength];

        for (int i = 0; i < tagsLength; i++)
            serializer.SerializeValue(ref tagIds[i]);

        // effectTriggers
        int triggersLength = effectTriggerDatas != null ? effectTriggerDatas.Length : 0;
        serializer.SerializeValue(ref triggersLength);

        if (serializer.IsReader)
            effectTriggerDatas = new int[triggersLength];

        for (int i = 0; i < triggersLength; i++)
            serializer.SerializeValue(ref effectTriggerDatas[i]);
    }
};
