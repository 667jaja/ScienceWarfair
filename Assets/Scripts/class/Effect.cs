using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Effect", menuName = "Effect")]
public abstract class Effect : ScriptableObject
{ 
    [field: SerializeField] public int effectId {get; private set;}
    public ActionData actionData { get; set;}
    //[field: SerializeField] public GameAction trigger { get; private set; }
    public abstract List<GameAction> effect { get;}

    // public void PerformEffect(ActionData actionData1)
    // {
    //     //foreach 
    //     actionData = actionData1;
    //     ActionManager.instance.Perform(effect);
    // }
}
