using System.Buffers.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "DrawCardsEffect", menuName = "Draw Cards Effect")]
public class DrawCardsEF : Effect
{
    
    [field: SerializeField] public int cardCount { get; private set; }
    [field: SerializeField] public Card originCard { private get; set; }

    public override GameAction effect
    {
        get
        {
            DrawCardsGA drawCardsGA = new DrawCardsGA(base.actionData.playerId, cardCount);
            return drawCardsGA;
        }
    }  


    // [field: SerializeField] public int placementCost { get; private set; }
    // [field: SerializeField] public int placementCost { get; private set; }
}
