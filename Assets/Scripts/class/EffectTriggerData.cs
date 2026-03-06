using System.Collections.Generic;
using UnityEngine;
public enum EffectTriggerType
{
    Placement = 0,
    StartTurn = 1,
    CardPlaced = 2
}
//GameAction.GetType() & typeof(GameAction) is potential solution to enum issue
[CreateAssetMenu(fileName = "EffectTriggerData", menuName = "Effect Trigger Data")]
public class EffectTriggerData : ScriptableObject
{
    //id
    [field: SerializeField] public int effectTriggerId {get; private set;}

    //main event
    [field: SerializeField] public EffectTriggerType effectTriggerType;
    [field: SerializeField] public List<Effect> effects {get; private set; }
    [field: SerializeField] public string effectDescription { get; private set; }

    //vars
    [field: SerializeField] public bool pre {get; set;}
    [field: SerializeField] public bool oneTimeUse {get; set;} = false;

    [field: SerializeField] public int countDown {get; set;} = 0;
    [field: SerializeField] public int countDownVal = 0;

    [field: SerializeField] public bool targetEnemyOnly = false;
    [field: SerializeField] public bool targetMeOnly = false;
}
