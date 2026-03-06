using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public struct EffectTriggerStruct : INetworkSerializable
{
    public int effectTriggerId;
    public int countDownVal;
    public int originPlayer;
    public int originUnitInstanceId;
    //public int originPosition;
    public bool triggerDisabled;
    public int[] effects;

    public EffectTriggerStruct(EffectTrigger effectTrigger)
    {  
        effectTriggerId = effectTrigger.effectTriggerData.effectTriggerId;
        countDownVal = effectTrigger.countDownVal;
        originPlayer = effectTrigger.originPlayer;
        originUnitInstanceId = effectTrigger.originUnitInstanceId;
        triggerDisabled = effectTrigger.triggerDisabled;
        effects = new int[effectTrigger.effects.Count];

        if (effectTrigger.effects != null && effectTrigger.effects.Count > 0)
        for (int i = 0; i < effectTrigger.effects.Count; i++)
        {
            if (effectTrigger.effects[i] != null) effects[i] = effectTrigger.effects[i].effectId;
        }
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref effectTriggerId);
        serializer.SerializeValue(ref countDownVal);
        serializer.SerializeValue(ref originPlayer);
        serializer.SerializeValue(ref originUnitInstanceId);
        serializer.SerializeValue(ref triggerDisabled);

        // effects
        int effectsLength = effects != null ? effects.Length : 0;
        serializer.SerializeValue(ref effectsLength);

        if (serializer.IsReader)
            effects = new int[effectsLength];

        for (int i = 0; i < effectsLength; i++)
            serializer.SerializeValue(ref effects[i]);
    }
}
