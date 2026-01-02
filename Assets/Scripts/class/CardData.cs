using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card Data")]

public class CardData : ScriptableObject
{    
    [field: SerializeField] public string title { get; private set; }
    [field: SerializeField] public string description { get; private set; }
    [field: SerializeField] public Sprite cardArt { get; private set; }
    [field: SerializeField] public bool isAction; 
    [field: SerializeField] public int placementCost { get; private set; }
    [field: SerializeField] public int iq { get; private set; }
    [field: SerializeField] public int health { get; private set; }
    [field: SerializeField] public bool noAttack { get; private set; }
    [field: SerializeField] public string cardEffect { get; private set; }
    // [field: SerializeField] public List<EffectTrigger> effectTriggers {get; private set; }
    [field: SerializeField] public EffectTrigger effectTriggers {get; private set; }
    [field: SerializeField] public List<Effect> effects {get; private set; }
    [field: SerializeField] public CardData containedCard {get; private set; }

}