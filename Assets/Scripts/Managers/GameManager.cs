using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //manages overarching elements of a game such as players, loss, victory, turn start, turn end
    public static GameManager instance;
    [SerializeField] private int playerCount = 2;    
    public List<Player> players = new List<Player>();
    public int currentPlayer;
    //public int perspective player
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        BeginGame();
    }
    private void BeginGame()
    {
        players = new();
        for (int i = 0; i < playerCount; i++)
        {
            players.Add(new Player(i));
            CardManager.instance.CreateDeck(i);
        }
        currentPlayer = 0;
    }
    public void TurnEndButton()
    {
        if (ActionManager.instance.isPerforming) return;
        else EndTurn();
    }
    private void EndTurn()
    {
        EndTurnGA endTurnGA = new(currentPlayer);
        ActionManager.instance.Perform(endTurnGA);
    }
    private IEnumerator EndTurnPerformer(EndTurnGA endTurnGA)
    {
        Debug.Log("turnEnd player id: " + endTurnGA.playerId);
        currentPlayer = (endTurnGA.playerId < players.Count-1)? endTurnGA.playerId + 1 : 0;
        yield return null;
    }
    private void StartTurn()
    {
        if (ActionManager.instance.isPerforming) return;

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
    }
}
