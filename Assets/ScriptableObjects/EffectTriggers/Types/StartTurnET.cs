// using UnityEngine;

// //[CreateAssetMenu(fileName = "StartTurnEffectTrigger", menuName = "Start Turn Effect Trigger")]
// public class StartTurnET : EffectTrigger
// {
//     [field: SerializeField] private bool oneTimeUse {get; set;} = false;
//     [field: SerializeField] private bool onlyMyTurn {get; set;} = true;
//     [field: SerializeField] private int countDown {get; set;} = 0;
//     [field: SerializeField] private int countDownVal = 0;
//     private bool triggerDisabled;
//     public override void PlacementEffect(ActionData actionData)
//     {
//         if (pre) ActionManager.SubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.PRE);
//         else ActionManager.SubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.POST);
//     }
//     public override void DestructionEffect()
//     {
//         if (pre) ActionManager.UnubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.PRE);
//         else ActionManager.UnubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.POST);
//     }
//     // public StartTurnET(Card originCard) : base(originCard){}

//     // private void OnEnable()
//     // {
//     //     if (pre) ActionManager.SubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.PRE);
//     //     else ActionManager.SubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.POST);
//     // }
//     // private void OnDisable()
//     // {
//     //     if (pre) ActionManager.UnubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.PRE);
//     //     else ActionManager.UnubscribeReaction<StartTurnGA>(Reaction, ReactionTiming.POST);
//     // }

//     private void Reaction(StartTurnGA startTurnGA)
//     {
//         ActionData actionData = new ActionData(base.originPlayer, new Vector2Int(0,0), null);
//         if ((startTurnGA.playerId == base.originPlayer || !onlyMyTurn) && !triggerDisabled)
//         {
//             if (countDownVal <= 0)
//             {
//                 base.TriggerEffect(actionData);
//                 if (oneTimeUse) triggerDisabled = true;
//                 countDownVal = countDown;
                
//             }
//             else
//             {
//                 countDown -= 1;
//             }
//         }
//     }
// }
