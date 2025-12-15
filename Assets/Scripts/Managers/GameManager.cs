using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //manages overarching elements of a game such as players, loss, victory, turn start, turn end
    public static GameManager instance;
    [SerializeField] private int playerCount = 2;    
    public List<Player> players = new List<Player>();
    public int currentPlayer;
    public int displayPlayer;
    public Slider moneySlider;

    public Slider plaSciencePointsSlider;
    public Slider oppSciencePointsSlider;

    //public int perspective player
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        displayPlayer = currentPlayer;
        BeginGame();
    }
    void Update()
    {
        displayPlayer = currentPlayer;
    }
    private void BeginGame()
    {
        players = new();
        for (int i = 0; i < playerCount; i++)
        {
            players.Add(new Player(i));
            players[i].money = 3;
            CardManager.instance.CreateDeck(i);
            CardManager.instance.DrawCards(i, 5);
        }
        
        UpdateMoneyUI(players[0]);
        currentPlayer = 0;
        StartTurn();
    }
    public void TurnEndButton()
    {
        //if (ActionManager.instance.isPerforming) return;
        EndTurn();
    }
    private void EndTurn()
    {
        EndTurnGA endTurnGA = new(currentPlayer);
        ActionManager.instance.Perform(endTurnGA);
    }
    private IEnumerator EndTurnPerformer(EndTurnGA endTurnGA)
    {
        Debug.Log("turnEnd player id: " + endTurnGA.playerId);

        //money
        players[currentPlayer].money += 2;
        //iq
        yield return LaneManager.instance.CountIqVisual();
        players[currentPlayer].sciencePoints += UnitManager.instance.CountPlayerIQ(currentPlayer);

        //currentplayer
        currentPlayer = GetNextPLayerId();
        Debug.Log("current player IQ: " + players[currentPlayer].sciencePoints);
        Debug.Log("next player IQ: " + players[GetNextPLayerId()].sciencePoints);
        yield return null;
    }
    private void StartTurn()
    {
        StartTurnGA startTurnGA = new(currentPlayer);
        ActionManager.instance.Perform(startTurnGA);
    }
    private IEnumerator StartTurnPerformer(StartTurnGA startTurnGA)
    {
        Debug.Log("turnStart player id: " + startTurnGA.playerId);
        GlobalUIUpdate();
        yield return null;
    }
    private void OnEnable()
    {
        ActionManager.AttachPerformer<EndTurnGA>(EndTurnPerformer);
        ActionManager.AttachPerformer<StartTurnGA>(StartTurnPerformer);
        ActionManager.SubscribeReaction<EndTurnGA>(EndTurnReaction, ReactionTiming.POST);

    }
    private void OnDisable()
    {
        ActionManager.DetachPerformer<EndTurnGA>();
        ActionManager.DetachPerformer<StartTurnGA>();
        ActionManager.UnubscribeReaction<EndTurnGA>(EndTurnReaction, ReactionTiming.POST);
    }
    private void EndTurnReaction(EndTurnGA endTurnGA)
    {
        Debug.Log("turn end detected");
        StartTurnGA startTurnGA = new(currentPlayer);
        ActionManager.instance.AddReaction(startTurnGA);
    }
    public void GlobalUIUpdate()
    {
        CardManager.instance.UpdateDeckUI();
        CardManager.instance.UpdateHandUI();
        UnitManager.instance.UpdateUnitUI();
        UpdateMoneyUI(players[displayPlayer]);
        UpdateSciencePointSliders();
        LaneManager.instance.UpdateLaneVisuals();
    }
    public void UpdateMoneyUI(Player updatePlayer = null)
    {
        if (updatePlayer == null) updatePlayer = players[displayPlayer];
        moneySlider.value = updatePlayer.money;
    } 
    public void UpdateSciencePointSliders()
    {
        plaSciencePointsSlider.value = players[currentPlayer].sciencePoints;
        oppSciencePointsSlider.value = players[GetNextPLayerId()].sciencePoints;
    }
    public int GetNextPLayerId(int playerId = -1)
    {
        if (playerId < 0) playerId = currentPlayer;
        return (playerId < players.Count - 1) ? playerId + 1 : 0;
    }
}
