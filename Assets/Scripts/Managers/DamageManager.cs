using System.Collections;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    //Controls card damage and death
    void OnEnable()
    {
        ActionManager.AttachPerformer<DealDamageGA>(DealDamagePerformer);
    }
    void OnDisable()
    {
        ActionManager.DetachPerformer<DealDamageGA>();
    }
    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)
    {
        int damageAmount = dealDamageGA.amount;
        Debug.Log("attacking...");
        yield return new WaitForSeconds(1);
        //yield return new WaitForSeconds(1);
        Debug.Log("dealt " + damageAmount + " damage");
    }
}
