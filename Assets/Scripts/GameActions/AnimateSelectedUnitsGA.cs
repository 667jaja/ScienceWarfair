using UnityEngine;

public class AnimateSelectedUnitsGA : GameAction
{
    public SpecialAnimation animVal;
    public float waitTimePer;
    public float waitTimeOver;
    
    public AnimateSelectedUnitsGA(SpecialAnimation animVal2, float waitTimeOver2, float waitTimePer2)
    {
        waitTimePer = waitTimePer2;
        waitTimeOver = waitTimeOver2;
        animVal = animVal2;
    }
}
