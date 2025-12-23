using UnityEngine;

[CreateAssetMenu(fileName = "GainMoneyEffect", menuName = "Gain Money Effect")]
public class GainMoneyEF : Effect
{
    [field: SerializeField] public int gainCount { get; private set; }
    [field: SerializeField] public Card originCard { private get; set; }

    public override GameAction effect
    {
        get
        {
            GainMoneyGA gainMoneyGA = new GainMoneyGA(base.actionData.playerId, gainCount);
            return gainMoneyGA;
        }
    }  
}
