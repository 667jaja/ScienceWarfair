
using UnityEngine;

[CreateAssetMenu(fileName = "Tag", menuName = "Tag")]
public class CardTag : ScriptableObject
{
    [field: SerializeField] public int tagId;
    [field: SerializeField] public Sprite tagImage;
    [field: SerializeField] public Color tagColor;
    [field: SerializeField] public string tagName;
}
