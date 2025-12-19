using UnityEngine;

//[CreateAssetMenu(fileName = "Effect", menuName = "Effect")]
public abstract class Effect : ScriptableObject
{ 
    public ActionData actionData { get; set;}
    //[field: SerializeField] public GameAction trigger { get; private set; }
    public abstract GameAction effect { get;}
}
