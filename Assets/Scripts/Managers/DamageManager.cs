using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    //Controls card damage and death
    void OnEnable()
    {
        ActionManager.AttachPerformer<DamageUnitGA>(DamageUnitPerformer);
        ActionManager.AttachPerformer<AttackLaneGA>(AttackLanePerformer);
        ActionManager.AttachPerformer<DestroyUnitGA>(DestroyUnitPerformer);
    }
    void OnDisable()
    {
        ActionManager.DetachPerformer<DamageUnitGA>();
        ActionManager.DetachPerformer<AttackLaneGA>();
        ActionManager.DetachPerformer<DestroyUnitGA>();
    }
    //deal damage to front unit in lane 
    private IEnumerator AttackLanePerformer(AttackLaneGA attackLaneGA)
    {
        int damageAmount = attackLaneGA.amount;
        //Debug.Log("attacking...");
        
        if (GameManager.instance.players[attackLaneGA.playerId].units[attackLaneGA.lane, 0] == null)
        {
            Debug.Log("no unit to attack");
        }
        else
        {
            //attack unit
            DamageUnitGA damageUnitGA = new(attackLaneGA.playerId, new Vector2Int(attackLaneGA.lane, 0), attackLaneGA.amount);
            ActionManager.instance.AddReaction(damageUnitGA);
            Debug.Log("dealt " + damageAmount + " damage to player " + attackLaneGA.playerId + " lane " + attackLaneGA.lane);
            
            if (attackLaneGA.playerId != GameManager.instance.displayPlayer) yield return new WaitForSeconds(LaneManager.instance.AttackAnimation(attackLaneGA.lane));
            else yield return new WaitForSeconds(LaneManager.instance.AttackDownAnimation(attackLaneGA.lane));
        }
    }
    //deal damage to one unit
    private IEnumerator DamageUnitPerformer(DamageUnitGA damageUnitGA)
    {   
        Card selectedUnit = GameManager.instance.players[damageUnitGA.playerId].units[damageUnitGA.position.x, damageUnitGA.position.y]; //get only reference
        int damageAmount = damageUnitGA.amount;
        yield return new WaitForSeconds(UnitManager.instance.DamageAnimation(damageUnitGA.playerId, damageUnitGA.position));

        if (selectedUnit.health <= damageAmount)
        {
            DestroyUnitGA destroyUnitGA = new(damageUnitGA.playerId, damageUnitGA.position);
            ActionManager.instance.AddReaction(destroyUnitGA);
        }
        else
        {
            GameManager.instance.players[damageUnitGA.playerId].units[damageUnitGA.position.x, damageUnitGA.position.y].health -= damageAmount;  
            UnitManager.instance.UpdateCardVisual(damageUnitGA.playerId, damageUnitGA.position);
        }


        // Debug.Log("dealt " + damageAmount + " damage to unit " + selectedUnit.title + " position " + damageUnitGA.position);
    }
    //destroy one unit
    private IEnumerator DestroyUnitPerformer(DestroyUnitGA destroyUnitGA)
    {
        Card selectedUnit = GameManager.instance.players[destroyUnitGA.playerId].units[destroyUnitGA.position.x, destroyUnitGA.position.y]; //get only reference
        yield return new WaitForSeconds(UnitManager.instance.DestroyAnimation(destroyUnitGA.playerId, destroyUnitGA.position));
        Debug.Log("unit Destroyed: " + selectedUnit.title);
        
        GameManager.instance.players[destroyUnitGA.playerId].units[destroyUnitGA.position.x, destroyUnitGA.position.y].Destruciton();

        Card containedCard = (selectedUnit.containedCard != null)? new Card(selectedUnit.containedCard): null;
        GameManager.instance.players[destroyUnitGA.playerId].units[destroyUnitGA.position.x, destroyUnitGA.position.y] = null;

        if (containedCard != null)
        {
            CreateUnitGA createUnitGA = new(destroyUnitGA.playerId, destroyUnitGA.position, containedCard);
            ActionManager.instance.AddReaction(createUnitGA);
        }

        UnitManager.instance.PushAllUnitsForward();
        UnitManager.instance.UpdateUnitUI();
        LaneManager.instance.UpdateLaneVisuals();
    }
    //deal damage to all units in lane 

}
