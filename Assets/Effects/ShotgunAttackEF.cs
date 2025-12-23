using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShotgunLaneEffect", menuName = "Shotgun Lane Effect")]
public class ShotgunAttackEF : Effect
{
    
    [field: SerializeField] public int attackDamage { get; private set; }
    [field: SerializeField] public Card originCard { private get; set; }

    public override List<GameAction> effect
    {
        get
        {
            int targetplayer = GameManager.instance.GetNextPlayerId(base.actionData.playerId);
            int remainingDamage = attackDamage;
            List<GameAction> actionList = new List<GameAction>(); 
            int i = 0;
            
            while (remainingDamage > 0)
            {
                if (GameManager.instance.players[targetplayer].units[base.actionData.position.x, i] != null)
                {
                    AttackLaneGA attackLaneGA = new AttackLaneGA(targetplayer, base.actionData.position.x, remainingDamage);
                    actionList.Add(attackLaneGA);
                    remainingDamage -= GameManager.instance.players[targetplayer].units[base.actionData.position.x, i].health;
                }
                else
                {
                    remainingDamage = 0;
                }
                i++;
            }

            return actionList;
        }
    }  

}

