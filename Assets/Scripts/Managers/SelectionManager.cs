using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;

    public List<Card> selectedHand = new();
    public List<Card> selectedDiscard = new();
    public List<Vector3Int> selectedBoard = new(); //x, y, playerId
    public List<Vector2Int> selectedLanes = new(); //x, playerId
    public bool endSelectionOverride;

    void Awake()
    {
        instance = this;
    }
    void OnEnable() 
    {
        ActionManager.AttachPerformer<SelectLanesGA>(SelectLanePerformer);
        ActionManager.AttachPerformer<SelectOpposingLanesGA>(SelectOpposingLanes);
        ActionManager.AttachPerformer<SelectUnitsGA>(SelectBoardPerformer);
        ActionManager.AttachPerformer<SelectUnitsInLanesGA>(SelectUnitsInSelectedLanes);
        ActionManager.AttachPerformer<SelectSpecificUnitsGA>(SelectSpecificUnitsPerformer);
        ActionManager.AttachPerformer<SelectCardsGA>(SelectHandPerformer);
    }
    void OnDisable()
    {
        ActionManager.DetachPerformer<SelectLanesGA>();
        ActionManager.DetachPerformer<SelectOpposingLanesGA>();
        ActionManager.DetachPerformer<SelectUnitsGA>();
        ActionManager.DetachPerformer<SelectUnitsInLanesGA>();
        ActionManager.DetachPerformer<SelectSpecificUnitsGA>();
        ActionManager.DetachPerformer<SelectCardsGA>();
    }
    public List<Vector2Int> LanesFromConditions(int playerId = -1)
    {
        List<Vector2Int> thingToReturn = new();
        for (int p = 0; p < GameManager.instance.players.Count; p++)
        {
            //check player
            if (p == playerId || playerId < 0)
            for (int i = 0; i < UnitManager.instance.columnCount; i++)
            {
                thingToReturn.Add(new Vector2Int(i, p));
            }
        }

        return thingToReturn;
    }
    public List<Vector3Int> PositionsFromConditions(int playerId = -1, int row = -1, CardTag tag = null, int minIq = -1, int maxIq = -1, int minHealth = -1, int maxHealth = -1, int minCost = -1, int maxCost = -1)
    {
        List<Vector3Int> thingToReturn = new();
        for (int p = 0; p < GameManager.instance.players.Count; p++)
        {
            //check player
            if (p == playerId || playerId < 0)
            for (int i = 0; i < UnitManager.instance.columnCount; i++)
            {
                for (int j = 0; j < UnitManager.instance.rowCount; j++)
                {
                    //every unit 
                    //check if exists and is in correct row
                    if ((row < 0 || row == j) && GameManager.instance.players[p].units[i,j] != null)
                    {
                        Card inspectedUnit = GameManager.instance.players[p].units[i,j];
                        bool addCurrent = true;
                        //check tags
                        if (tag != null)
                        {
                            addCurrent = false;
                            foreach (CardTag cardTag in inspectedUnit.tags)
                                if (cardTag.tagId == tag.tagId)
                                {
                                    addCurrent = true;
                                    break;
                                }
                        }
                        //check stats
                        //iq
                        if ((minIq >= 0 && inspectedUnit.iq < minIq) || (maxIq >= 0 && inspectedUnit.iq > maxIq))
                            addCurrent = false;
                        //health
                        if ((minHealth >= 0 && inspectedUnit.health < minHealth) || (maxHealth >= 0 && inspectedUnit.health > maxHealth))
                            addCurrent = false;
                        //cost
                        if ((minCost >= 0 && inspectedUnit.placementCost < minCost) || (maxCost >= 0 && inspectedUnit.placementCost > maxCost))
                            addCurrent = false;
                        
                        if (addCurrent) thingToReturn.Add(new Vector3Int(i,j,p));
                    }
                }
            }
        }

        return thingToReturn;
    }

    public IEnumerator SelectBoardPerformer(SelectUnitsGA selectUnitsGA)
    {
        endSelectionOverride = false;
        selectedBoard = new();

        //make sure there are enough things to select
        int validUnitsCount = 0;
        foreach (Vector3Int pos in selectUnitsGA.validPos)
        {
            if (GameManager.instance.players[pos.z].units[pos.x, pos.y] != null) validUnitsCount++;
        }
        if (validUnitsCount < selectUnitsGA.selectCount) selectUnitsGA.selectCount = validUnitsCount;

        //make units selectable
        if (GameManager.instance.displayPlayer == selectUnitsGA.inputPlayerId)
            UnitManager.instance.MakeUnitsSelectable(selectUnitsGA.validPos);

        //wait for player to select
        float overrideTimer = 0;
        while (selectedBoard.Count < selectUnitsGA.selectCount || overrideTimer>15)
        {
            if (endSelectionOverride) overrideTimer += Time.deltaTime;
            yield return null;
        }

        //carryover selection to other player
        if (RelayManager.instance != null && GameManager.instance.displayPlayer == selectUnitsGA.inputPlayerId)
        {
            OnlineManager.instance.InputSelection();
        }

        //make units not selectable
        UnitManager.instance.MakeAllUnitsNotSelectable();


        endSelectionOverride = false;
    } 
    //selects all units in selected lanes
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
    //selects all lanes opposing selected lanes
    public IEnumerator SelectOpposingLanes(SelectOpposingLanesGA selectOpposingLanesGA)
    {
        List<Vector2Int> toAdd = new();
        foreach (Vector2Int lane in selectedLanes)
        {
            toAdd.Add(new Vector2Int(lane.x, GameManager.instance.GetNextPlayerId(lane.y)));
        }
        selectedLanes.AddRange(toAdd);
        yield return null;
    }
    public IEnumerator SelectLanePerformer(SelectLanesGA selectLanesGA)
    {
        selectedLanes = new();
        endSelectionOverride = false;
        //make lanes selectable
        if (GameManager.instance.displayPlayer == selectLanesGA.inputPlayerId)
        foreach (Vector2Int item in selectLanesGA.validLanes)
        {
            LaneManager.instance.MakeLaneSelectable(item);
        }

        //wait for player to select
        float overrideTimer = 0;
        while (selectedLanes.Count < selectLanesGA.selectCount || overrideTimer>15)
        {
            if (endSelectionOverride) overrideTimer += Time.deltaTime;
            yield return null;
        }

        //carryover selection to other player
        if (RelayManager.instance != null && GameManager.instance.displayPlayer == selectLanesGA.inputPlayerId)
        {
            OnlineManager.instance.InputSelection();
        }

        //make lanes not selectable
        LaneManager.instance.LaneSelectOff();
        endSelectionOverride = false;
    } 
    public IEnumerator SelectHandPerformer(SelectCardsGA selectCardsGA)
    {
        endSelectionOverride = false;
        CardManager.instance.UpdateHandUI();
        selectedHand = new();

        //make sure there are enough things to select
        int validCardsCount = 0;
        foreach (Card item in selectCardsGA.validCards)
        {
            if (CardManager.instance.FindCardInHand(selectCardsGA.inputPlayerId, item.cardInstanceId, item.cardData.CardDataId) != null) validCardsCount++;
        }

        if (validCardsCount < selectCardsGA.selectCount) selectCardsGA.selectCount = validCardsCount;

        //make cards selectable
        if (GameManager.instance.displayPlayer == selectCardsGA.inputPlayerId)
        foreach (Card item in selectCardsGA.validCards)
        {
            CardManager.instance.MakeHandSelectable(item);
        }

        //wait for player to select
        float overrideTimer = 0;
        while (selectedHand.Count < selectCardsGA.selectCount  || overrideTimer>15)
        {
            if (endSelectionOverride) overrideTimer += Time.deltaTime;
            yield return null;
        }

        //carryover selection to other player
        if (RelayManager.instance != null && GameManager.instance.displayPlayer == selectCardsGA.inputPlayerId)
        {
            OnlineManager.instance.InputSelection();
        }

        //make cards not selectable
        CardManager.instance.MakeHandUnselectable();

        endSelectionOverride = false;
    } 
    public IEnumerator SelectDiscardPerformer(List<Card> avaliableOptions, int selectCount)
    {
        selectedDiscard = new();
        yield return null;
    }

    public IEnumerator SelectSpecificUnitsPerformer(SelectSpecificUnitsGA selectSpecificUnitsGA)
    {
        if (selectSpecificUnitsGA.newSelection) selectedBoard = new();
        foreach (int id in selectSpecificUnitsGA.cardIds)
        {
            if (UnitManager.instance.GetUnitByInstanceId(id) != null) selectedBoard.Add(UnitManager.instance.GetBoardPosByInstanceId(id));
        }
        yield return null;
    }
}
