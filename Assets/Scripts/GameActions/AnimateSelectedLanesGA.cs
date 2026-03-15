using UnityEngine;

public class AnimateSelectedLanesGA : GameAction
{
    public SpecialAnimation animVal;
    public float waitTimePer;
    public float waitTimeOver;
    public AnimateSelectedLanesGA(SpecialAnimation animVal2, float waitTimeOver2, float waitTimePer2)
    {
        waitTimePer = waitTimePer2;
        waitTimeOver = waitTimeOver2;
        animVal = animVal2;
    }
}
