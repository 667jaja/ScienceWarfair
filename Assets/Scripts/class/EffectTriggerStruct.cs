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

    public EffectTriggerStruct(EffectTrigger effectTrigger)
    {  
        effectTriggerId = effectTrigger.effectTriggerData.effectTriggerId;
        countDownVal = effectTrigger.countDownVal;
        originPlayer = effectTrigger.originPlayer;
        originUnitInstanceId = effectTrigger.originUnitInstanceId;
        triggerDisabled = effectTrigger.triggerDisabled;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref effectTriggerId);
        serializer.SerializeValue(ref countDownVal);
        serializer.SerializeValue(ref originPlayer);
        serializer.SerializeValue(ref originUnitInstanceId);
        serializer.SerializeValue(ref triggerDisabled);
    }
}
