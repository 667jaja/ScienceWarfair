using UnityEngine;

public class GiveUnitEffectGA : GameAction
{
    public int playerId;
    public Vector2Int position;
    public EffectTriggerData ET;
    public Card savedCard;
    public GiveUnitEffectGA(int playerId2, Vector2Int position2, EffectTriggerData ET2, Card savedCard2 = null)
    {
        playerId = playerId2;
        position = position2;
        ET = ET2;
        savedCard = savedCard2;
    }
}
