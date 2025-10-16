using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card Data")]

public class CardData : ScriptableObject
{
    [field: SerializeField] public string title { get; private set; }
    [field: SerializeField] public Sprite cardArt { get; private set; }
    [field: SerializeField] public int placementCost { get; private set; }
    [field: SerializeField] public string cardEffect { get; private set; }
}