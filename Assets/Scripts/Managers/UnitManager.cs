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

    public bool TryPlaceCard(int playerId, int lane, Card card)
    {
        if (ActionManager.instance.isPerforming || GameManager.instance.players[playerId].units[lane, rowCount-1] != null) return false;

        PlaceUnitGA placeUnitGA = new(playerId, lane, card);
        ActionManager.instance.Perform(placeUnitGA);
        Debug.Log("Placement success: Player:" + playerId + " Lane:" + lane + " Card:" + card.title);

        return true;
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
        Debug.Log("unit placement performed");

        GameManager.instance.players[placeUnitGA.playerId].hand.Remove(placeUnitGA.playedCard);
        CardManager.instance.UpdateHandUI();
        CreateUnitReaction(placeUnitGA.playerId, placeUnitGA.lane, placeUnitGA.playedCard);

        //frontend
        yield return null;
    }
    private IEnumerator CreateUnitPerformer(CreateUnitGA createUnitGA)
    {
        Debug.Log("unit creation performed");

        //CardVisual card = Instantiate(cardVisual, playerHand.position, Quaternion.identity);

        //backend
        //for every row
        for (int i = 0; i < rowCount; i++)
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
        }

        //frontend
        UpdateUnitUI();
        yield return null;
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
            bool isCurrentPlayer = player.id == GameManager.instance.currentPlayer;
            int playerAdd = (isCurrentPlayer) ? 0 : columnCount;
            //empty UI objects

            

            //replace UI objects
            for (int i = playerAdd; i < columnCount + playerAdd; i++)
            {
                //for every column

                //all the new card visuals in this column
                List<CardVisual> laneVisuals = new List<CardVisual>();

                //always create lowest on screen unit last
                //this isn't a pretty way of doing this
                if (!isCurrentPlayer)
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
}