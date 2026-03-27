using UnityEngine;

public class BoardToHandSelectedGA : GameAction
{
    public int recieverId;
    public bool ownersHand;
    public bool refundOwner;

    public BoardToHandSelectedGA(int recieverId2, bool ownersHand2 = false, bool refundOwner2 = false)
    {
        recieverId = recieverId2;
        ownersHand = ownersHand2;
        refundOwner = refundOwner2;
    }
}
