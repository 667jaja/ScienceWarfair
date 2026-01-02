using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShotgunLaneEffect", menuName = "Shotgun Lane Effect")]
public class ShotgunAttackEF : Effect
{
    
    [field: SerializeField] public int attackDamage { get; private set; }

    public override List<GameAction> effect
    {
        get
        {
            int targetplayer = GameManager.instance.GetNextPlayerId(base.actionData.originPlayerId);
            int remainingDamage = attackDamage;
            List<GameAction> actionList = new List<GameAction>(); 
            int i = 0;
            
            while (remainingDamage > 0)
            {
                Debug.Log("player: " + targetplayer +" position: " + base.actionData.originPosition.x + " " + i);
                if (GameManager.instance.players[targetplayer].units[base.actionData.originPosition.x, i] != null && i < UnitManager.instance.rowCount)
                {
                    AttackLaneGA attackLaneGA = new AttackLaneGA(targetplayer, base.actionData.originPosition.x, remainingDamage);
                    actionList.Add(attackLaneGA);
                    remainingDamage -= GameManager.instance.players[targetplayer].units[base.actionData.originPosition.x, i].health;
                    Debug.Log("remainingDamage: " + remainingDamage);

                    if (remainingDamage > 0 && GameManager.instance.players[targetplayer].units[base.actionData.originPosition.x, i].containedCard != null)
                    {
                        attackLaneGA = new AttackLaneGA(targetplayer, base.actionData.originPosition.x, remainingDamage);
                        actionList.Add(attackLaneGA);
                        remainingDamage -= GameManager.instance.players[targetplayer].units[base.actionData.originPosition.x, i].containedCard.health;
                        Debug.Log("remainingDamage: " + remainingDamage);
                    }
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

