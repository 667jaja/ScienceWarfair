using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public enum SpecialAnimation
{
    Null = 0,
    DestroyMoon = 1,
    Potion = 2,
    Laugh = 3,
    Business = 4,
    Mozeltov = 5,
    Pesticide = 6,
}
public class SpecialAnimationManager : MonoBehaviour
{
    public static SpecialAnimationManager instance;
    public List<string> animNames;
    public List<float> animLength;
    public float longestAnimLength;

    // Special Animation
    public GameObject specialAnimationPrefab;
    public Transform specialAnimationTransform;

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        ActionManager.AttachPerformer<SpecialAnimationGA>(SpecialAnimationPerformer);
        ActionManager.AttachPerformer<AnimateSelectedLanesGA>(AnimateSelectedLanes);
        ActionManager.AttachPerformer<AnimateSelectedUnitsGA>(AnimateSelectedUnits);
    }
    private void OnDisable()
    {
        ActionManager.DetachPerformer<SpecialAnimationGA>();
        ActionManager.DetachPerformer<AnimateSelectedLanesGA>();
        ActionManager.DetachPerformer<AnimateSelectedUnitsGA>();
    }

    public float AnimationLength(SpecialAnimation animVal)
    {
        return animLength[(int)animVal];
    }
    public IEnumerator AnimateSelectedLanes(AnimateSelectedLanesGA animateSelectedLanesGA)
    {
        int i = 0;
        foreach (Vector2Int vector in SelectionManager.instance.selectedLanes)
        {
            i++;
            //add "waitTimePer"to the wait time of every animation played
            //add "waitTimeOver" to the wait time of the last animation played
            if (i == SelectionManager.instance.selectedLanes.Count) animateSelectedLanesGA.waitTimePer += animateSelectedLanesGA.waitTimeOver;
            SpecialAnimationGA specialAnimationGA = new SpecialAnimationGA(animateSelectedLanesGA.animVal, new Vector2Int(vector.x,0), vector.x, vector.y, animateSelectedLanesGA.waitTimePer);
            ActionManager.instance.AddReaction(specialAnimationGA);
        }
        yield return null;
    }
    public IEnumerator AnimateSelectedUnits(AnimateSelectedUnitsGA animateSelectedUnitsGA)
    {
        int i = 0;
        foreach (Vector3Int vector in SelectionManager.instance.selectedBoard)
        {
            i++;
            //add "waitTimePer"to the wait time of every animation played
            //add "waitTimeOver" to the wait time of the last animation played
            if (i == SelectionManager.instance.selectedBoard.Count) animateSelectedUnitsGA.waitTimePer += animateSelectedUnitsGA.waitTimeOver;
            SpecialAnimationGA specialAnimationGA = new SpecialAnimationGA(animateSelectedUnitsGA.animVal, new Vector2Int(vector.x, vector.y), vector.x, vector.z, animateSelectedUnitsGA.waitTimePer);
            ActionManager.instance.AddReaction(specialAnimationGA);
        }
        yield return null;
    }
    public IEnumerator SpecialAnimationPerformer(SpecialAnimationGA specialAnimationGA)
    {
        Debug.Log("Playing Special animation " + (int)specialAnimationGA.animVal);

        if (specialAnimationGA.animVal != SpecialAnimation.Null)
        {
            //find animations
            string AnimationPlayed = animNames[(int)specialAnimationGA.animVal];

            //find position of lane and unit
            bool isEnemy = specialAnimationGA.playerId != GameManager.instance.displayPlayer;
            Vector2 unitPos = Vector2.zero;
            Vector2 lanePos = Vector2.zero;
            if (specialAnimationGA.unitLocation.x >= 0 && specialAnimationGA.unitLocation.y >= 0 && specialAnimationGA.playerId >= 0)
            {
                unitPos = UnitManager.instance.GetBoardPos(new Vector3Int(specialAnimationGA.unitLocation.x, specialAnimationGA.unitLocation.y, specialAnimationGA.playerId));
            }
            if (specialAnimationGA.laneLocation >= 0 && specialAnimationGA.playerId >= 0)
            {
                lanePos = UnitManager.instance.GetLanePos(new Vector2Int(specialAnimationGA.laneLocation, specialAnimationGA.playerId));
            }
            
            SpecialAnimationController newAnimController = Instantiate(specialAnimationPrefab, specialAnimationTransform).GetComponent<SpecialAnimationController>();
            newAnimController.SpecialAnimation(AnimationPlayed, unitPos, lanePos, isEnemy, longestAnimLength);

            yield return new WaitForSeconds(specialAnimationGA.waitTime);
        }
    }
}
