using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class LaneManager : MonoBehaviour
{
    public static LaneManager instance;
    [SerializeField] private List<LaneVisual> laneVisuals;
    [SerializeField] private List<LaneVisual> EnemyLaneVisuals;
    [SerializeField] private LaneVisual ActionCardLane;
    public List<Transform> lanePositions;

    [SerializeField] private List<Collider2D> UnitPlacementArea;
    [SerializeField] private List<GameObject> UnitPlacement;
    [SerializeField] private List<Collider2D> ActionPlacementArea;
    [SerializeField] private List<GameObject> ActionPlacement;
    //ui
    [SerializeField] private Slider iqAddSlider;
    //[SerializeField] private Slider iqAddSliderEnemy;

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
        iqAddSlider.value = 0;
        PlaceActionToggle(false);
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
        ActionCardLane.Initiate(true, -3);
        lanePositions.Add(ActionCardLane.transform);
    }
    public IEnumerator CountIqVisual()
    {
        int currentPlayerSP = GameManager.instance.players[GameManager.instance.currentPlayer].sciencePoints;
        int sciencePointsSum = currentPlayerSP;
        //if (currentPlayer == displayPlayer) {all}
        int i = 0;
        foreach(LaneVisual lane in laneVisuals)
        {
            // + sciencePointsSum
            lane.UpdateVisual(UnitManager.instance.CountLaneIQ(GameManager.instance.displayPlayer,i));
            
            sciencePointsSum += UnitManager.instance.CountLaneIQ(GameManager.instance.currentPlayer, lane.lanePos);
            iqAddSlider.value = sciencePointsSum;
            // if (currentPlayer != DisplayPlayer)
            //iqAddSliderEnemy
            yield return new WaitForSeconds(CountIqAnimation(i));
            i++;
        }
    }
    public IEnumerator AddIqVisual(int playerId, int AddAmount)
    {
        // if (currentPlayer != DisplayPlayer)
        //iqAddSliderEnemy
        iqAddSlider.value = GameManager.instance.players[GameManager.instance.currentPlayer].sciencePoints + AddAmount;
        yield return new WaitForSeconds(countIqAnimationLength);
    }
    public void UpdateLaneVisuals()
    {
        foreach(LaneVisual lane in laneVisuals)
        {
            lane.UpdateVisual();
        }
        iqAddSlider.value = 0;
    }
    public void PlaceActionToggle(bool isPlacingAction)
    {
        foreach (Collider2D item in ActionPlacementArea)
        {
            item.enabled = isPlacingAction;
        }
        foreach (GameObject item in ActionPlacement)
        {
            item.SetActive(isPlacingAction);
        }
        foreach (Collider2D item in UnitPlacementArea)
        {
            item.enabled = !isPlacingAction;
        }
        foreach (GameObject item in UnitPlacement)
        {
            item.SetActive(!isPlacingAction);
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
