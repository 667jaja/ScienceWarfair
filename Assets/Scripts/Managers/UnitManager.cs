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
    [SerializeField] private int columnCount = 3;
    //[SerializeField] private int enemyColumnCount = 3;
    [SerializeField] private int rowCount = 3;


    [SerializeField] private CardVisual unitVisual;
    [SerializeField] private Transform[] laneTransforms = new Transform[6];//[6];
    [SerializeField] private CardVisual[,] unitVisuals = new CardVisual[6, 3];//[6, 3]; //all unit visuals currently in play
    //[SerializeField] private CardVisual[] units = new CardVisual[3];


    //animations
    [SerializeField] private string placementAnimationName = "Placed";
    [SerializeField] private float placementAnimationLength;
    [SerializeField] private string destroyAnimationName;
    [SerializeField] private float destroyAnimationLength;
    [SerializeField] private string damageAnimationName;
    [SerializeField] private float damageAnimationLength;

    void Awake()
    {
        instance = this;
        unitVisuals = new CardVisual[columnCount * 2, rowCount];
    }
    private void OnEnable()
    {
        ActionManager.AttachPerformer<PlaceUnitGA>(PlaceUnitPerformer);
        ActionManager.AttachPerformer<CreateUnitGA>(CreateUnitPerformer);
    }
    private void OnDisable()
    {
        ActionManager.DetachPerformer<PlaceUnitGA>();
        ActionManager.DetachPerformer<CreateUnitGA>();
    }
    public void PlaceCard(int playerId, int lane, Card card)
    {
        //if (ActionManager.instance.isPerforming || GameManager.instance.players[playerId].units[lane, rowCount-1] != null) return false;
        //if (GameManager.instance.players[playerId].units[lane, rowCount-1] != null) return false;

        PlaceUnitGA placeUnitGA = new(playerId, lane, card);
        ActionManager.instance.Perform(placeUnitGA);

        // return true;
    }
    public void CreateUnit(int playerId, int lane, Card card)
    {
        if (ActionManager.instance.isPerforming) return;

        CreateUnitGA createUnitGA = new(playerId, lane, card);
        ActionManager.instance.Perform(createUnitGA);
    }
    public void CreateUnitReaction(int playerId, int lane, Card card)
    {
        CreateUnitGA createUnitGA = new(playerId, lane, card);
        ActionManager.instance.AddReaction(createUnitGA);
    }
    private IEnumerator PlaceUnitPerformer(PlaceUnitGA placeUnitGA)
    {
        if (GameManager.instance.players[placeUnitGA.playerId].units[placeUnitGA.lane, rowCount-1] == null && placeUnitGA.playedCard.placementCost <= GameManager.instance.players[placeUnitGA.playerId].money)
        {
            Debug.Log("Placement success: Player:" + placeUnitGA.playerId + " Lane:" + placeUnitGA.lane + " Card:" + placeUnitGA.playedCard.title);
            GameManager.instance.players[placeUnitGA.playerId].hand.Remove(placeUnitGA.playedCard);
            CardManager.instance.UpdateHandUI();
            GameManager.instance.players[placeUnitGA.playerId].money -= placeUnitGA.playedCard.placementCost;
            GameManager.instance.UpdateMoneyUI();
            CreateUnitReaction(placeUnitGA.playerId, placeUnitGA.lane, placeUnitGA.playedCard);

            //attack lane
            AttackLaneGA attackLaneGA = new((placeUnitGA.playerId < GameManager.instance.players.Count-1)? placeUnitGA.playerId+1: 0, placeUnitGA.lane, 1);
            ActionManager.instance.AddReaction(attackLaneGA);
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

        //backend
        //for every row
        int i = 0;
        for (i = 0; i < rowCount; )
        {
            if (GameManager.instance.players[createUnitGA.playerId].units[createUnitGA.lane, i] == null)
            {
                GameManager.instance.players[createUnitGA.playerId].units[createUnitGA.lane, i] = createUnitGA.playedCard;
                break;
            }
            if (i == rowCount - 1)
            {
                Debug.Log("Lanes Full");
            }
            i++;
        }

        //frontend
        UpdateUnitUI();
        PlacementAnimation(GameManager.instance.currentPlayer, new Vector2Int(createUnitGA.lane, i));
        yield return new WaitForSeconds(1/3);
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
    public void UpdateUnitUI()
    {
        // UpdateLaneUI(laneTransforms[0], 0, GameManager.instance.players[GameManager.instance.currentPlayer].lane1);
        for (int i = 0; i < GameManager.instance.players.Count * columnCount; i++)
        {
            //for every column
            for (int j = 0; j < rowCount; j++)
            {
                // Debug.Log("player: " + player.id + " index: " + i + ":" + j);
                // Debug.Log("units height: " + unitVisuals.GetLength(0) + " units width: " + unitVisuals.GetLength(1));

                //for every row
                if (unitVisuals[i, j] != null)
                {
                    Destroy(unitVisuals[i, j].gameObject);
                    Debug.Log("Destroyed Visuals: " + i + ": " + j);
                }
                // else
                // {
                //     Debug.Log("Lane: " + i + ": " + j + " empty");
                // }
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
    private void UnitTriggerAnimation(string AnimName, Vector2Int index)
    {
        unitVisuals[index.x, index.y].GetComponent<Animator>().SetTrigger(AnimName);
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

        // if (isEnemy)
        // {
        //     i = unitsInLane;
        //     foreach (CardVisual item in cardVisuals)
        //     {
        //         i--;
        //         //if (item != null) item.transform.position = new Vector2(LanePos.position.y, LanePos.position.x);// - laneHeight / 2 + (laneHeight / (unitsInLane + 1)) * (i + 1));
        //         if (item != null) item.transform.position = new Vector2(LanePos.position.x, LanePos.position.y- laneHeight / 2 + (laneHeight / (unitsInLane + 1)) * (i + 1));// ;
        //     }
        // }
        // else
        // {
        //     i = -1;

        //     foreach (CardVisual item in cardVisuals)
        //     {
        //         i++;
        //         //if (item != null) item.transform.position = new Vector2(LanePos.position.y, LanePos.position.x);// - laneHeight / 2 + (laneHeight / (unitsInLane + 1)) * (i + 1));
        //         if (item != null) item.transform.position = new Vector2(LanePos.position.x, LanePos.position.y- laneHeight / 2 + (laneHeight / (unitsInLane + 1)) * (i + 1));// ;
        //     }
        // }

    }
    public void DebugUnitPositions()   
    {
        foreach (Player player in GameManager.instance.players)
        {
            int playerAdd = (player.id == GameManager.instance.currentPlayer) ? 0 : columnCount;

            for (int i = playerAdd; i < columnCount + playerAdd; i++)
            {
                //for every column

                for (int j = 0; j < rowCount; j++)
                {
                    //for every row
                    // Debug.Log("BACKEND::" +" Player: "+ ((player.id == GameManager.instance.currentPlayer) ? "current" : "other") +" X:" + (i-playerAdd) + " Y:" + j + "contents: " + ((player.units[i - playerAdd, j] == null) ? "empty" : player.units[i - playerAdd, j]));
                    // Debug.Log("FRONTEND:: X:" + i + " Y:" + j + "contents: " + ((unitVisuals[i, j] == null) ? "empty" : unitVisuals[i, j]));
                    if (player.units[i - playerAdd, j] != null) Debug.Log("BACKEND::" +" Player: "+ ((player.id == GameManager.instance.currentPlayer) ? "current" : "other") +" X:" + (i-playerAdd) + " Y:" + j + "contents: " + player.units[i - playerAdd, j]);
                    if (unitVisuals[i, j] != null) Debug.Log("FRONTEND:: X:" + i + " Y:" + j + "contents: " + unitVisuals[i, j]);
                }
            }
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
}