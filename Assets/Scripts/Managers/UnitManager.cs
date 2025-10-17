using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;

    //lane sizing
    [SerializeField] private float maxLaneHeight;
    [SerializeField] private float laneHeightScaleRate;


    [SerializeField] private CardVisual cardVisual;
    [SerializeField] private Transform[] laneTransforms = new Transform[6];
    //[SerializeField] private CardVisual[,] units = new CardVisual[6,3];
    [SerializeField] private CardVisual[] units = new CardVisual[3];

    void Awake()
    {
        instance = this;
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
        if (ActionManager.instance.isPerforming || GameManager.instance.players[playerId].lane1[2] != null) return false;

        PlaceUnitGA placeUnitGA = new(playerId, lane, card);
        ActionManager.instance.Perform(placeUnitGA);

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
        // if (GameManager.instance.players[drawCardGA.playerId].deck.Count < 1)
        // {
        //     ShuffleDiscardIntoDeck(drawCardGA.playerId);
        // }
        int i = 0;
        foreach (Card item in GameManager.instance.players[createUnitGA.playerId].lane1)
        {
            if (item == null)
            {
                GameManager.instance.players[createUnitGA.playerId].lane1[i] = createUnitGA.playedCard;
                break;
            }
            i++;
        }

        //frontend
        UpdateUnitUI();
        yield return null;
    }
    public void UpdateUnitUI()
    {
        UpdateLaneUI(laneTransforms[0], 0, GameManager.instance.players[GameManager.instance.currentPlayer].lane1);
    }
    private void UpdateLaneUI(Transform LanePos, int lane, Card[] cards)
    {
        foreach (CardVisual item in units)
        {
            if (item != null) Destroy(item.gameObject);
        }
        units = new CardVisual[3];
        int i = 0;
        foreach (Card item in cards)
        {
            if (item != null)
            {
                CardVisual newCardVisual = Instantiate(cardVisual, LanePos);
                newCardVisual.Initiate(item);
                units[i] = (newCardVisual);
                i++;
            }
        }

        UpdateLanePositioning(LanePos, units);
    } 
    private void UpdateLanePositioning(Transform LanePos, CardVisual[] cardVisuals)
    {
        int i = -1;
        float unitsInLane = cardVisuals.Length;
        float laneHeight = -(maxLaneHeight * laneHeightScaleRate) / (unitsInLane + laneHeightScaleRate) + maxLaneHeight;
        foreach (CardVisual item in cardVisuals)
        {
            i++;
            if (item != null) item.transform.position = new Vector2(LanePos.position.y, LanePos.position.x-laneHeight/2 + (laneHeight/(unitsInLane+1))*(i+1));
        }
    }
}
