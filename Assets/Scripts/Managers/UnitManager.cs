using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using NUnit.Framework;
public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;

    //lane sizing
    [SerializeField] private float maxLaneHeight;
    [SerializeField] private float laneHeightScaleRate;
    //
    public int columnCount = 3;
    //[SerializeField] private int enemyColumnCount = 3;
    public int rowCount = 3;

    [SerializeField] private CardVisual actionVisual;
    [SerializeField] private CardVisual unitVisual;
    [SerializeField] private Transform[] laneTransforms = new Transform[6];//[6];
    [SerializeField] private CardVisual[,] unitVisuals = new CardVisual[6, 3];//[6, 3]; //all unit visuals currently in play

    //[SerializeField] private CardVisual[] units = new CardVisual[3];


    //animations
    [SerializeField] private string placementAnimationName = "Placed";
    [SerializeField] private float placementAnimationLength;
    [SerializeField] private float actionPlayAnimationLength;
    [SerializeField] private string destroyAnimationName;
    [SerializeField] private float destroyAnimationLength;
    [SerializeField] private string damageAnimationName;
    [SerializeField] private float damageAnimationLength;

    void Awake()
    {
        instance = this;
        unitVisuals = new CardVisual[columnCount * 2, rowCount];
    }
    void Start()
    {
        for (int i = 0; i < laneTransforms.Length; i++)
        {
            laneTransforms[i] = LaneManager.instance.lanePositions[i];
        }
    }
    private void OnEnable()
    {
        //play
        ActionManager.AttachPerformer<PlaceUnitGA>(PlaceUnitPerformer);
        ActionManager.AttachPerformer<CreateUnitGA>(CreateUnitPerformer);
        ActionManager.AttachPerformer<PlayActionGA>(PlayActionPerformer);

        //move
        ActionManager.AttachPerformer<MoveUnitGA>(MoveUnitPerformer);
        ActionManager.AttachPerformer<BoardToHandGA>(BoardToHandPerformer);
        ActionManager.AttachPerformer<BoardToHandSelectedGA>(BoardToHandSelectedPerformer);

        //change
        ActionManager.AttachPerformer<ChangeStatsUnitGA>(ChangeStatsUnitPerformer);
        ActionManager.AttachPerformer<ChangeStatsSelectedGA>(ChangeStatsSelectedPerformer);
        ActionManager.AttachPerformer<GiveUnitEffectGA>(GiveUnitEffectPerformer);
        ActionManager.AttachPerformer<GiveSelectedEffectGA>(GiveSelectedEffectPerformer);
        ActionManager.AttachPerformer<GiveSelectedUnitedEffectGA>(GiveSelectedUnitedEffectPerformer);
    }
    private void OnDisable()
    {
        //play
        ActionManager.DetachPerformer<PlaceUnitGA>();
        ActionManager.DetachPerformer<CreateUnitGA>();
        ActionManager.DetachPerformer<PlayActionGA>();

        //move
        ActionManager.DetachPerformer<MoveUnitGA>();
        ActionManager.DetachPerformer<BoardToHandGA>();
        ActionManager.DetachPerformer<BoardToHandSelectedGA>();

        //change
        ActionManager.DetachPerformer<ChangeStatsUnitGA>();
        ActionManager.DetachPerformer<ChangeStatsSelectedGA>();
        ActionManager.DetachPerformer<GiveUnitEffectGA>();
        ActionManager.DetachPerformer<GiveSelectedUnitedEffectGA>();
    }

    public void PlayAction(int playerId, Card card) 
    {
        PlayActionGA playActionGA = new(playerId, new Card(card.cardData));
        ActionManager.instance.Perform(playActionGA);
    }
    public void PlaceCard(int playerId, int lane, Card card)
    {
        //if (ActionManager.instance.isPerforming || GameManager.instance.players[playerId].units[lane, rowCount-1] != null) return false;
        //if (GameManager.instance.players[playerId].units[lane, rowCount-1] != null) return false;

        PlaceUnitGA placeUnitGA = new(playerId, lane, new Card(card.cardData));
        ActionManager.instance.Perform(placeUnitGA);

        // return true;
    }
    public void CreateUnit(int playerId, Vector2Int position, Card card)
    {
        if (ActionManager.instance.isPerforming) return;

        CreateUnitGA createUnitGA = new(playerId, position, card);
        ActionManager.instance.Perform(createUnitGA);
    }
    public void CreateUnitReaction(int playerId, Vector2Int position, Card card)
    {
        CreateUnitGA createUnitGA = new(playerId, position, card);
        createUnitGA.description = card.title + " is placed";
        ActionManager.instance.AddReaction(createUnitGA);
    }
    private IEnumerator PlayActionPerformer(PlayActionGA playActionGA)
    {

        // check if player has points remove from hand
        if (GameManager.instance.players[playActionGA.playerId].actionPoints > 0)
        {
            if (CardManager.instance.RemoveCard(playActionGA.playerId, playActionGA.playedCard))
            {
                Debug.Log("Action Play success: Player:" + playActionGA.playerId + " Card:" + playActionGA.playedCard.title);
                
                //backend
                playActionGA.playedCard.subET(new ActionData(playActionGA.playerId, new Vector2Int (-1,-1), playActionGA.playedCard));
                playActionGA.playedCard.PlacementAbility(new ActionData(playActionGA.playerId, new Vector2Int (-1,-1), playActionGA.playedCard));
                GameManager.instance.players[playActionGA.playerId].actionPoints -= 1;
                GameManager.instance.UpdateActionPointUI();

                //frontend
                CardManager.instance.UpdateHandUI();
                CardVisual newCardVisual = Instantiate(actionVisual, LaneManager.instance.lanePositions[columnCount*2]);
                newCardVisual.Initiate(playActionGA.playedCard);
                yield return new WaitForSeconds(actionPlayAnimationLength);

                if (newCardVisual != null)
                {
                    Destroy(newCardVisual.gameObject);
                }
            }
            else
            {
                Debug.Log("Action use failed, card not found");  
            }
        }
        else
        {
            Debug.Log("Action Card failed");
            GameManager.instance.GlobalUIUpdate();
        }

        //frontend
        yield return null;
    }
    private IEnumerator PlaceUnitPerformer(PlaceUnitGA placeUnitGA)
    {

        // find open spot
        bool unitPlacementSuccess = false;
        int i = 0;
        for (i = 0; i < rowCount; )
        {
            if (GameManager.instance.players[placeUnitGA.playerId].units[placeUnitGA.lane, i] == null)
            {
                unitPlacementSuccess = true;
                break;
            }
            if (i == rowCount - 1)
            {
                unitPlacementSuccess = false;
                Debug.Log("Lanes Full");
            }
            i++;
        }

        // check that we can afford
        if (unitPlacementSuccess && placeUnitGA.playedCard.PlacementCost <= GameManager.instance.players[placeUnitGA.playerId].Money)
        {
            // remove from hand
            if (CardManager.instance.RemoveCard(placeUnitGA.playerId, placeUnitGA.playedCard))
            {
                Debug.Log("Placement success: Player:" + placeUnitGA.playerId + " Lane:" + placeUnitGA.lane + " Card:" + placeUnitGA.playedCard.title);
                Vector2Int firstOpenSpot = new Vector2Int(placeUnitGA.lane, i);

                CardManager.instance.UpdateHandUI();
                GameManager.instance.players[placeUnitGA.playerId].Money -= placeUnitGA.playedCard.PlacementCost;
                GameManager.instance.UpdateMoneyUI();


                if (unitPlacementSuccess) CreateUnitReaction(placeUnitGA.playerId, firstOpenSpot, placeUnitGA.playedCard);

                //attack lane
                if (!placeUnitGA.playedCard.noAttack)
                {
                    AttackLaneGA attackLaneGA = new((placeUnitGA.playerId < GameManager.instance.players.Count-1)? placeUnitGA.playerId+1: 0, placeUnitGA.lane, 1);
                    ActionManager.instance.AddReaction(attackLaneGA);
                }
            }
            else
            {
                Debug.Log("unit placement failed, card not found");
                GameManager.instance.GlobalUIUpdate();
            }
        }
        else
        {
            Debug.Log("unit placement failed");
            GameManager.instance.GlobalUIUpdate();
        }

        //frontend
        yield return null;
    }
    private IEnumerator CreateUnitPerformer(CreateUnitGA createUnitGA)
    {
        Debug.Log("unit creation performed");

        //CardVisual card = Instantiate(cardVisual, playerHand.position, Quaternion.identity);

        PushAllUnitsForward();
        bool unitCreationSuccess = false;

        unitCreationSuccess = AddUnit(createUnitGA.playerId, createUnitGA.position, createUnitGA.playedCard);

        if (unitCreationSuccess)
        {
            createUnitGA.playedCard.PlacementAbility(new ActionData(createUnitGA.playerId, new Vector2Int (createUnitGA.position.x, createUnitGA.position.y), createUnitGA.playedCard));
        }

        //frontend
        UpdateUnitUI();

        if (unitCreationSuccess)
        {
            LaneManager.instance.UpdateLaneVisuals();
            PlacementAnimation(GameManager.instance.currentPlayer, new Vector2Int(createUnitGA.position.x, createUnitGA.position.y));
            yield return new WaitForSeconds(1/3);
        }
    }
    public bool AddUnit(int playerId, Vector2Int position, Card AddedCard)
    {
        bool unitAddSuccess = false;

        // if target space empty use it :)
        if (GameManager.instance.players[playerId].units[position.x, position.y] == null)
        {
            GameManager.instance.players[playerId].units[position.x, position.y] = AddedCard;
            GameManager.instance.players[playerId].units[position.x, position.y].subET(new ActionData(playerId, position, AddedCard));
            unitAddSuccess = true;
        }
        // else, check backmost space, if empty move all behind back 1
        else if (GameManager.instance.players[playerId].units[position.x, rowCount-1] == null)
        {
            for (int j = rowCount-1; j > position.y; j--)
            {
                //for every row (except the last)
                if (GameManager.instance.players[playerId].units[position.x, j] == null && GameManager.instance.players[playerId].units[position.x, j-1] != null)
                {
                    GameManager.instance.players[playerId].units[position.x, j] = GameManager.instance.players[playerId].units[position.x, j-1];
                    GameManager.instance.players[playerId].units[position.x, j-1] = null;
                }
            }
            GameManager.instance.players[playerId].units[position.x, position.y] = AddedCard;
            GameManager.instance.players[playerId].units[position.x, position.y].subET(new ActionData(playerId, position, AddedCard));

            unitAddSuccess = true;
        }
        // else, pack it up we lost
        else
        {
            unitAddSuccess = false;
            Debug.Log("Lanes Full");
        }
        if (unitAddSuccess)
        {
            PushAllUnitsForward();
            UpdateUnitUI();
            GameManager.instance.UpdateSciencePointsUI();
        }
        return unitAddSuccess;
    }

    private IEnumerator MoveUnitPerformer(MoveUnitGA moveUnitGA)
    {
        Card selectedUnit = GameManager.instance.players[moveUnitGA.playerId].units[moveUnitGA.originPosition.x, moveUnitGA.originPosition.y]; //get only reference

        if (selectedUnit != null)
        {
            RemoveUnit(moveUnitGA.playerId, moveUnitGA.originPosition);

            bool moveValid = AddUnit(moveUnitGA.playerId, moveUnitGA.destinationPosition, selectedUnit);

            if (!moveValid) AddUnit(moveUnitGA.playerId, moveUnitGA.originPosition, selectedUnit);

            GameManager.instance.UpdateSciencePointsUI();
            LaneManager.instance.UpdateLaneVisuals();
        }

        yield return null;
    }
    private IEnumerator BoardToHandPerformer(BoardToHandGA boardToHandGA)
    {
        Card selectedUnit = GameManager.instance.players[boardToHandGA.position.z].units[boardToHandGA.position.x, boardToHandGA.position.y]; //get only reference

        if (selectedUnit != null)
        {
            RemoveUnit(boardToHandGA.position.z, new Vector2Int(boardToHandGA.position.x, boardToHandGA.position.y));

            CardManager.instance.AddtoHand(boardToHandGA.recieverId, selectedUnit.cardData);

            UpdateUnitUI();
            PushAllUnitsForward();
            LaneManager.instance.UpdateLaneVisuals();
            GameManager.instance.UpdateSciencePointsUI();
        }

        yield return null;
    }
    private IEnumerator BoardToHandSelectedPerformer(BoardToHandSelectedGA boardToHandSelectedGA)
    {
        foreach (Vector3Int vector in SelectionManager.instance.selectedBoard)
        {
            BoardToHandGA boardToHandGA =  new BoardToHandGA((boardToHandSelectedGA.ownersHand)? vector.z : boardToHandSelectedGA.recieverId, vector);
            ActionManager.instance.AddReaction(boardToHandGA);

            if (boardToHandSelectedGA.refundOwner)
            {
                GainMoneyGA gainMoneyGA =  new GainMoneyGA(vector.z, GameManager.instance.players[vector.z].units[vector.x, vector.y].PlacementCost);
                ActionManager.instance.AddReaction(gainMoneyGA);
            }

        }
        yield return null;
    }
    private IEnumerator ChangeStatsUnitPerformer(ChangeStatsUnitGA changeStatsUnitGA)
    {
        Card selectedUnit = GameManager.instance.players[changeStatsUnitGA.playerId].units[changeStatsUnitGA.position.x, changeStatsUnitGA.position.y]; //get only reference
        
        selectedUnit.Iq += changeStatsUnitGA.iqChange;
        selectedUnit.Health += changeStatsUnitGA.heathChange;
        selectedUnit.PlacementCost += changeStatsUnitGA.costChange;
        if (changeStatsUnitGA.tag != null && !selectedUnit.tags.Contains(changeStatsUnitGA.tag)) selectedUnit.tags.Add(changeStatsUnitGA.tag);

        UpdateCardVisual(changeStatsUnitGA.playerId, changeStatsUnitGA.position);
        LaneManager.instance.UpdateLaneVisuals();
        GameManager.instance.UpdateSciencePointsUI();
        yield return null;
    }
    private IEnumerator ChangeStatsSelectedPerformer(ChangeStatsSelectedGA changeStatsSelectedGA)
    {
        foreach (Vector3Int vector in SelectionManager.instance.selectedBoard)
        {
            ChangeStatsUnitGA changeStatsUnitGA =  new ChangeStatsUnitGA(vector.z, new Vector2Int(vector.x, vector.y), changeStatsSelectedGA.iqChange, changeStatsSelectedGA.heathChange, changeStatsSelectedGA.costChange, changeStatsSelectedGA.tag);
            ActionManager.instance.AddReaction(changeStatsUnitGA);
        }
        yield return null;
    }
    private IEnumerator GiveUnitEffectPerformer(GiveUnitEffectGA giveUnitEffectGA)
    {
        Card selectedUnit = GameManager.instance.players[giveUnitEffectGA.playerId].units[giveUnitEffectGA.position.x, giveUnitEffectGA.position.y]; //get only reference
        if (selectedUnit != null)
        {
            EffectTrigger addedEF = new EffectTrigger(new ActionData(giveUnitEffectGA.playerId, giveUnitEffectGA.position, selectedUnit), giveUnitEffectGA.ET);
            if (giveUnitEffectGA.savedCard != null) addedEF.savedCardInstanceId = giveUnitEffectGA.savedCard.cardInstanceId;
            selectedUnit.effectTriggers.Add(addedEF);

            UpdateCardVisual(giveUnitEffectGA.playerId, giveUnitEffectGA.position);
            LaneManager.instance.UpdateLaneVisuals();
            yield return null;
        }
    }
    private IEnumerator GiveSelectedEffectPerformer(GiveSelectedEffectGA giveSelectedEffectGA)
    {
        foreach (Vector3Int vector in SelectionManager.instance.selectedBoard)
        {
            GiveUnitEffectGA giveUnitEffectGA =  new GiveUnitEffectGA(vector.z, new Vector2Int(vector.x, vector.y), giveSelectedEffectGA.ET, giveSelectedEffectGA.savedCard);
            ActionManager.instance.AddReaction(giveUnitEffectGA);
        }
        yield return null;
    }
    private IEnumerator GiveSelectedUnitedEffectPerformer(GiveSelectedUnitedEffectGA giveSelectedUnitedEffectGA)
    {
        foreach (Vector3Int vector in SelectionManager.instance.selectedBoard)
        {
            foreach (Vector3Int mector in SelectionManager.instance.selectedBoard)
            {
                if (mector != vector)
                {
                    Card mectorCard = GameManager.instance.players[mector.z].units[mector.x, mector.y];
                    GiveUnitEffectGA giveUnitEffectGA =  new GiveUnitEffectGA(vector.z, new Vector2Int(vector.x, vector.y), giveSelectedUnitedEffectGA.ET, mectorCard);
                    ActionManager.instance.AddReaction(giveUnitEffectGA);
                }
            }

        }
        yield return null;
    }

    public void PushAllUnitsForward()
    {
        foreach (Player player in GameManager.instance.players)
        {
            //for every player
            for (int n = 0; n < rowCount-1; n++)
            {
                for (int i = 0; i < columnCount; i++)
                {
                    //for every column
                    for (int j = 0; j < rowCount-1; j++)
                    {
                        //for every row (except the last)
                        if (player.units[i,j] == null && player.units[i,j+1] != null)
                        {
                            player.units[i,j] = player.units[i,j+1];
                            player.units[i,j+1] = null;
                        }
                    }
                }
                //repeat as many times as there are rows -1 
                //so that even if there is one just unit in the back it will move to the front
            }
        }
    }
    public void RemoveUnit(int playerId, Vector2Int pos)
    {
        GameManager.instance.players[playerId].units[pos.x, pos.y].UnsubET();
        GameManager.instance.players[playerId].units[pos.x, pos.y] = null;
    }

    public Card GetUnitByInstanceId(int instanceId)
    {
        //Card foundCard = null;
        foreach (Player player in GameManager.instance.players)
        {
            //for every player
            for (int i = 0; i < columnCount; i++)
            {
                //for every column
                for (int j = 0; j < rowCount; j++)
                {
                    //for every row
                    if (player.units[i,j] != null && player.units[i,j].cardInstanceId == instanceId)
                    {
                        Debug.Log("found unit by id at " + player.name +": " + i+":"+j);
                        return player.units[i,j];
                    }
                }
            }
        }
        Debug.Log("GetUnitByInstanceId failed. No unit found");
        return null;
    }
    public Vector3Int GetBoardPosByInstanceId(int instanceId)
    {
        //Card foundCard = null;
        foreach (Player player in GameManager.instance.players)
        {
            //for every player
            for (int i = 0; i < columnCount; i++)
            {
                //for every column
                for (int j = 0; j < rowCount; j++)
                {
                    //for every row
                    if (player.units[i,j] != null && player.units[i,j].cardInstanceId == instanceId)
                    {
                        Debug.Log("found unit by id at " + player.name +": " + i+":"+j);
                        return new Vector3Int(i,j, player.id);
                    }
                }
            }
        }
        Debug.Log("GetUnitByInstanceId failed. No unit found");
        return new Vector3Int(-1,-1,-1);
    }
    public Vector2 GetBoardPos(Vector3Int position)
    {
        bool isDisplay = position.z == GameManager.instance.displayPlayer;
        int playerAdd = isDisplay? 0 : columnCount;
        Transform LanePos = laneTransforms[position.x+playerAdd];
        Vector2 foundLoc = Vector2.zero;

        //find number of units in lane
        int unitsInLane = 0;
        int i = 0;
        for (i = 0; i < rowCount; i++)
        {
            if (GameManager.instance.players[position.z].units[position.x, i] != null) unitsInLane++;
        }

        //use to calculate lane height
        float laneHeight = -(maxLaneHeight * laneHeightScaleRate) / (unitsInLane + laneHeightScaleRate) + maxLaneHeight;

        //units physically below target (enemy units go up)
        int unitsBelow = isDisplay? unitsInLane-position.y : position.y;

        foundLoc = new Vector2(LanePos.position.x, LanePos.position.y - laneHeight / 2 + (laneHeight / (unitsInLane + 1)) * (unitsBelow + 1));

        return foundLoc;
    }
    public Vector2 GetLanePos(Vector2Int position)
    {
        int playerAdd = (position.y == GameManager.instance.displayPlayer)? 0 : columnCount;
        Transform LanePos = laneTransforms[position.x+playerAdd];


        return LanePos.transform.position;
    }
    public void UpdateUnitUI()
    {
        // UpdateLaneUI(laneTransforms[0], 0, GameManager.instance.players[GameManager.instance.currentPlayer].lane1);
        for (int i = 0; i < GameManager.instance.players.Count * columnCount; i++)
        {
            //for every column
            for (int j = 0; j < rowCount; j++)
            {
                //for every row
                if (unitVisuals[i, j] != null)
                {
                    Destroy(unitVisuals[i, j].gameObject);
                }

            }
        }
        unitVisuals = new CardVisual[columnCount * 2, rowCount];
        foreach (Player player in GameManager.instance.players)
        {
            //for units from a player who is not the current player, add the amount of columns to the column index number
            bool isCurrentDisplayPlayer = player.id == GameManager.instance.displayPlayer;
            int playerAdd = (isCurrentDisplayPlayer) ? 0 : columnCount;
            //empty UI objects

            //replace UI objects
            for (int i = playerAdd; i < columnCount + playerAdd; i++)
            {
                //for every column

                //all the new card visuals in this column
                List<CardVisual> laneVisuals = new List<CardVisual>();

                //always create lowest on screen unit last
                //this isn't a pretty way of doing this
                if (!isCurrentDisplayPlayer)
                {
                    for (int j = rowCount - 1; j > -1; j--)
                    {
                        //for every row
                        
                        if (player.units[i - playerAdd, j] != null)
                        {
                            CardVisual newCardVisual = Instantiate(unitVisual, laneTransforms[i]);
                            newCardVisual.Initiate(player.units[i - playerAdd, j]);
                            unitVisuals[i, j] = newCardVisual;
                            laneVisuals.Add(newCardVisual);
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < rowCount; j++)
                    {
                        //for every row
                        if (player.units[i - playerAdd, j] != null)
                        {
                            CardVisual newCardVisual = Instantiate(unitVisual, laneTransforms[i]);
                            newCardVisual.Initiate(player.units[i - playerAdd, j]);
                            unitVisuals[i, j] = newCardVisual;
                            laneVisuals.Add(newCardVisual);
                        }
                    }
                }


                    UpdateLanePositioning(laneTransforms[i], laneVisuals);
            }
        }

    }
    private void UpdateLanePositioning(Transform LanePos, List<CardVisual> cardVisuals)
    {
        int i = -1;
        int unitsInLane = cardVisuals.Count;
        float laneHeight = -(maxLaneHeight * laneHeightScaleRate) / (unitsInLane + laneHeightScaleRate) + maxLaneHeight;

        i = unitsInLane;
        foreach (CardVisual item in cardVisuals)
        {
            i--;
            //if (item != null) item.transform.position = new Vector2(LanePos.position.y, LanePos.position.x);// - laneHeight / 2 + (laneHeight / (unitsInLane + 1)) * (i + 1));
            if (item != null) item.transform.position = new Vector2(LanePos.position.x, LanePos.position.y- laneHeight / 2 + (laneHeight / (unitsInLane + 1)) * (i + 1));// ;
        }
    }
    public void UpdateAllCardVisuals()
    {
        foreach (Player player in GameManager.instance.players)
        {
            bool isCurrentDisplayPlayer = player.id == GameManager.instance.displayPlayer;
            int playerAdd = (isCurrentDisplayPlayer) ? 0 : columnCount;

            for (int i = 0; i < columnCount; i++)
            {
                //for every column
                for (int j = 0; j < rowCount; j++)
                {
                    //for every row
                    if (unitVisuals[i+playerAdd, j] != null)
                    {
                        UpdateCardVisual(player.id, new Vector2Int(i,j));
                    }
                }
            }
        }
    }
    public void UpdateCardVisual(int playerId, Vector2Int position)
    {
        if (playerId != GameManager.instance.displayPlayer)
        {
            position = new Vector2Int(position.x + columnCount, position.y);
        }
        unitVisuals[position.x, position.y].UpdateVisuals();
    }

    private void UnitTriggerAnimation(string AnimName, Vector2Int index)
    {
        if (unitVisuals[index.x, index.y] != null)
        {
            unitVisuals[index.x, index.y].GetComponent<Animator>().SetTrigger(AnimName);
        }
    }
    public float PlacementAnimation(int playerId, Vector2Int position)
    {
        if (playerId != GameManager.instance.displayPlayer)
        {
            position = new Vector2Int(position.x + columnCount, position.y);
        }
        UnitTriggerAnimation(placementAnimationName, position);
        return placementAnimationLength;
    }
    public float DestroyAnimation(int playerId, Vector2Int position)
    {
        if (playerId != GameManager.instance.displayPlayer)
        {
            position = new Vector2Int(position.x + columnCount, position.y);
        }
        Debug.Log("animName: " + destroyAnimationName + " position: " + position.x +", "+ position.y);
        UnitTriggerAnimation(destroyAnimationName, position);
        return destroyAnimationLength;
    }
    public float DamageAnimation(int playerId, Vector2Int position)
    {
        if (playerId != GameManager.instance.displayPlayer)
        {
            position = new Vector2Int(position.x + columnCount, position.y);
        }
        UnitTriggerAnimation(damageAnimationName, position);
        return damageAnimationLength;
    }
    public void MakeUnitsSelectable(List<Vector3Int> selectedUnitPositions)
    {
        foreach(Vector3Int unitPosition in selectedUnitPositions)
        {
            int playerAdd = (unitPosition.z != GameManager.instance.displayPlayer)? columnCount : 0;
            Vector2Int index = new Vector2Int(playerAdd+unitPosition.x, unitPosition.y);
            if (unitVisuals[index.x, index.y] != null)
            {
                unitVisuals[index.x, index.y].SetSelectable(true);
            }
        }
    }
    public void MakeAllUnitsNotSelectable()
    {
        for (int i = 0; i < columnCount*2; i++)
        {
            //for every column
            for (int j = 0; j < rowCount; j++)
            {
                //for every row
                if (unitVisuals[i, j] != null)
                {
                    unitVisuals[i, j].SetSelectable(false);
                }
            }
        }
    }
    public void UnitSelected(CardVisual selectedUnit)
    {
        Vector3Int unitLocation = new Vector3Int(-1,-1);

        foreach (Player player in GameManager.instance.players)
        {
            bool isCurrentDisplayPlayer = player.id == GameManager.instance.displayPlayer;
            int playerAdd = (isCurrentDisplayPlayer) ? 0 : columnCount;

            for (int i = 0; i < columnCount; i++)
            {
                //for every column
                for (int j = 0; j < rowCount; j++)
                {
                    //for every row
                    if (unitVisuals[i+playerAdd, j] == selectedUnit)
                    {
                        unitLocation = new Vector3Int(i,j,player.id);
                    }
                }
            }
        }
        
        if (unitLocation.x >= 0 && unitLocation.y >= 0) SelectionManager.instance.selectedBoard.Add(unitLocation);
        else Debug.Log("Selected Unit Not Found On Board");
    }
    public int CountPlayerIQ(int playerid)
    {
        int sciencePoints = 0;
        for (int i = 0; i < columnCount; i++)
        {
            // for every column
            sciencePoints += CountLaneIQ(playerid, i);
        }
        return sciencePoints;
    }
    public int CountLaneIQ(int playerid, int lane)
    {
        int sciencePoints = 0;
        for (int i = 0; i < columnCount; i++)
        {
            Card inspectedCard = GameManager.instance.players[playerid].units[lane, i];
            //for every row
            if (inspectedCard != null)
            {
                sciencePoints += inspectedCard.Iq;
            }
        }
        return sciencePoints;
    }
}
    // public void DebugUnitPositions()   
    // {
    //     foreach (Player player in GameManager.instance.players)
    //     {
    //         int playerAdd = (player.id == GameManager.instance.currentPlayer) ? 0 : columnCount;

    //         for (int i = playerAdd; i < columnCount + playerAdd; i++)
    //         {
    //             //for every column

    //             for (int j = 0; j < rowCount; j++)
    //             {
    //                 //for every row
    //                 // Debug.Log("BACKEND::" +" Player: "+ ((player.id == GameManager.instance.currentPlayer) ? "current" : "other") +" X:" + (i-playerAdd) + " Y:" + j + "contents: " + ((player.units[i - playerAdd, j] == null) ? "empty" : player.units[i - playerAdd, j]));
    //                 // Debug.Log("FRONTEND:: X:" + i + " Y:" + j + "contents: " + ((unitVisuals[i, j] == null) ? "empty" : unitVisuals[i, j]));
    //                 if (player.units[i - playerAdd, j] != null) Debug.Log("BACKEND::" +" Player: "+ ((player.id == GameManager.instance.currentPlayer) ? "current" : "other") +" X:" + (i-playerAdd) + " Y:" + j + "contents: " + player.units[i - playerAdd, j]);
    //                 if (unitVisuals[i, j] != null) Debug.Log("FRONTEND:: X:" + i + " Y:" + j + "contents: " + unitVisuals[i, j]);
    //             }
    //         }
    //     }
    // }