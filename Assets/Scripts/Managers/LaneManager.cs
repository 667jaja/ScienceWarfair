using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public static LaneManager instance;
    [SerializeField] private List<LaneVisual> laneVisuals;
    [SerializeField] private List<LaneVisual> EnemyLaneVisuals;
    public List<Transform> lanePositions;
    //animations
    [SerializeField] private string attackAnimationName;
    [SerializeField] private float attackAnimationLength;
    [SerializeField] private string attackDownAnimationName;
    [SerializeField] private float attackDownAnimationLength;
    [SerializeField] private string countIqAnimationName;
    [SerializeField] private float countIqAnimationLength;

    void Awake()
    {
        instance = this;
        intitiateLanes();
    }
    private void intitiateLanes()
    {
        int i = 0;
        foreach(LaneVisual lane in laneVisuals)
        {

            lane.Initiate(true, i);
            lanePositions.Add(lane.gameObject.transform);
            i++;
        }
        i = 0;
        foreach(LaneVisual lane in EnemyLaneVisuals)
        {
            lane.Initiate(false, i);
            lanePositions.Add(lane.gameObject.transform);
            i++;
        }
    }
    public IEnumerator CountIqVisual()
    {
        //if (currentPlayer == displayPlayer) {all}
        int i = 0;
        //int sciencePointsSum = 0;
        foreach(LaneVisual lane in laneVisuals)
        {
            // + sciencePointsSum
            lane.UpdateVisual(UnitManager.instance.CountLaneIQ(GameManager.instance.displayPlayer,i));
            
             
            yield return new WaitForSeconds(CountIqAnimation(i));
            i++;
        }
        
    }
    public void UpdateLaneVisuals()
    {
        foreach(LaneVisual lane in laneVisuals)
        {
            lane.UpdateVisual();
        }
    }
    private void LaneTriggerAnimation(string AnimName, int index)
    {
        laneVisuals[index].GetComponent<Animator>().SetTrigger(AnimName);
    }
    public float AttackAnimation(int position)
    {
        LaneTriggerAnimation(attackAnimationName, position);
        return attackAnimationLength;
    }
    public float AttackDownAnimation(int position)
    {
        LaneTriggerAnimation(attackDownAnimationName, position);
        return attackDownAnimationLength;
    }
    public float CountIqAnimation(int position)
    {
        LaneTriggerAnimation(countIqAnimationName, position);
        return countIqAnimationLength;
    }
}
