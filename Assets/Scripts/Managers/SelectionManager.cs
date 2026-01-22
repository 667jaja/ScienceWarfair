using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;

    public List<Card> selectedHand;
    public List<Card> selectedDiscard;
    public List<Vector3Int> selectedBoard; //x, y, playerId
    public List<Vector2Int> selectedLanes; //x, playerId

    void Awake()
    {
        instance = this;
    }
    void OnEnable()
    {
        ActionManager.AttachPerformer<SelectLanesGA>(SelectLanePerformer);
        ActionManager.AttachPerformer<SelectUnitsInLanesGA>(SelectUnitsInSelectedLanes);
        ActionManager.AttachPerformer<SelectUnitsGA>(SelectBoardPerformer);
    }
    void OnDisable()
    {
        ActionManager.DetachPerformer<SelectLanesGA>();
    }
    public List<Vector2Int> EnemyLanes(int playerId)
    {
        if (playerId == 0)
        {
            return new List<Vector2Int>{new Vector2Int (0,1), new Vector2Int (1,1), new Vector2Int (2,1)};
        }
        else
        {
            return new List<Vector2Int>{new Vector2Int (0,0), new Vector2Int (1,0), new Vector2Int (2,0)};
        }
    }
    public IEnumerator SelectBoardPerformer(SelectUnitsGA selectUnitsGA)
    {
        selectedBoard = new();
        yield return null;
    } 
    public IEnumerator SelectUnitsInSelectedLanes(SelectUnitsInLanesGA selectUnitsInLanesGA)
    {
        selectedBoard = new();
        foreach (Vector2Int lane in selectedLanes)
        {
            for (int i = UnitManager.instance.rowCount-1; i >= 0; i--)
            {
                if (GameManager.instance.players[lane.y].units[lane.x, i] != null)
                {
                    selectedBoard.Add(new Vector3Int(lane.x, i, lane.y));
                }
            }
        }
        yield return null;
    }
    public IEnumerator SelectLanePerformer(SelectLanesGA selectLanesGA)
    {
        selectedLanes = new();
        foreach (Vector2Int item in selectLanesGA.validLanes)
        {
            LaneManager.instance.MakeLaneSelectable(item);
        }
        while (selectedLanes.Count < selectLanesGA.selectCount)
        {
            yield return null;
        }
        LaneManager.instance.LaneSelectOff();
    } 
    public IEnumerator SelectHandPerformer(List<Card> avaliableOptions, int selectCount)
    {
        selectedHand = new();
        yield return null;
    } 
    public IEnumerator SelectDiscardPerformer(List<Card> avaliableOptions, int selectCount)
    {
        selectedDiscard = new();
        yield return null;
    }
}
