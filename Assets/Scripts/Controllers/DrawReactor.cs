using UnityEngine;

public class DrawReactor : MonoBehaviour
{
    private void OnEnable()
    {
        ActionManager.SubscribeReaction<DrawCardGA>(drawCardReaction, ReactionTiming.POST);
    }
    private void OnDisable()
    {
        ActionManager.UnubscribeReaction<DrawCardGA>(drawCardReaction, ReactionTiming.POST);
    }

    private void drawCardReaction(DrawCardGA drawCardGA)
    {
        Debug.Log("card draw detected");
        DealDamageGA myAttack = new(3);
        ActionManager.instance.AddReaction(myAttack);
    }
}
