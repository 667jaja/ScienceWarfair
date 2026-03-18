using UnityEngine;

public class GiveSelectedEffectGA : GameAction
{
    public EffectTriggerData ET;
    public Card savedCard;
    public GiveSelectedEffectGA(EffectTriggerData ET2, Card savedCard2 = null)
    {
        ET = ET2;
        savedCard = savedCard2;
    }
}
