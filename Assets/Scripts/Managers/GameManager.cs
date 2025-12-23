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
    public int maxSciencePoints;
    public Slider moneySlider;

    public Slider plaSciencePointsSlider;
    public Slider oppSciencePointsSlider;

    //money&Cards
    [SerializeField] private int startingMoney;
    [SerializeField] private int turnOrderBonusMoney = 1;
    [SerializeField] private int startingCards;
    [SerializeField] private int turnOrderBonusCards = 1;
    [SerializeField] private int moneyGain = 2;
    [SerializeField] private int cardGain = 2;
    [SerializeField] private int maxMoney;


    [SerializeField] private float endOfTurnWait = 0.2f;

    //public int perspective player
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        displayPlayer = currentPlayer;
        //BeginGame();
    }
    void Update()
    {
        displayPlayer = currentPlayer;
    }
    public void BeginGame()
    {
        players = new();
        for (int i = 0; i < playerCount; i++)
        {
            players.Add(new Player(i));
            players[i].maxMoney = maxMoney;
            players[i].Money = startingMoney + i*turnOrderBonusMoney;
            CardManager.instance.CreateDeck(i);
            CardManager.instance.DrawCards(i, startingCards + i*turnOrderBonusCards);
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
        CardManager.instance.CurrentPlayerDrawCards(cardGain);

        EndTurnGA endTurnGA = new(currentPlayer);
        ActionManager.instance.Perform(endTurnGA);
    }
    private IEnumerator EndTurnPerformer(EndTurnGA endTurnGA)
    {
        Debug.Log("turnEnd player id: " + endTurnGA.playerId);

        //money
        //players[currentPlayer].money += moneyGain;
        yield return GainMoneyPerformer(new GainMoneyGA(currentPlayer, moneyGain));

        //cards
        //CardManager.instance.CurrentPlayerDrawCards(cardGain);

        //iq
        yield return LaneManager.instance.CountIqVisual();
        players[currentPlayer].sciencePoints += UnitManager.instance.CountPlayerIQ(currentPlayer);
        UpdateSciencePointSliders();

        //stall
        yield return new WaitForSeconds(endOfTurnWait);

        //currentplayer
        currentPlayer = GetNextPlayerId();

        if (HotseatScreenController.instance == null)
        {
            StartTurnGA startTurnGA = new(currentPlayer);
            ActionManager.instance.AddReaction(startTurnGA);
        }
        else
        {
            yield return HotseatScreenController.instance.OnTurnEnd();
        }
        // Debug.Log("current player IQ: " + players[currentPlayer].sciencePoints);
        // Debug.Log("next player IQ: " + players[GetNextPlayerId()].sciencePoints);
        yield return null;
    }
    public void StartTurn()
    {
        StartTurnGA startTurnGA = new(currentPlayer);
        ActionManager.instance.Perform(startTurnGA);
    }
    private IEnumerator StartTurnPerformer(StartTurnGA startTurnGA)
    {
        Debug.Log("turnStart player id: " + startTurnGA.playerId);
        players[currentPlayer].actionPoints = 1;
        GlobalUIUpdate();
        yield return null;
    }
    private IEnumerator GainMoneyPerformer(GainMoneyGA gainMoneyGA)
    {
        players[gainMoneyGA.playerId].Money += gainMoneyGA.gainCount;
        UpdateMoneyUI(players[displayPlayer]);
        yield return null;
    }
    private IEnumerator GainActionPointsPerformer(GainActionPointsGA gainActionPointsGA)
    {
        players[gainActionPointsGA.playerId].actionPoints += gainActionPointsGA.gainCount;
        yield return null;
    }
    private void OnEnable()
    {
        ActionManager.AttachPerformer<EndTurnGA>(EndTurnPerformer);
        ActionManager.AttachPerformer<StartTurnGA>(StartTurnPerformer);
        ActionManager.AttachPerformer<GainMoneyGA>(GainMoneyPerformer);
        ActionManager.AttachPerformer<GainActionPointsGA>(GainActionPointsPerformer);
        //ActionManager.SubscribeReaction<EndTurnGA>(EndTurnReaction, ReactionTiming.POST);

    }
    private void OnDisable()
    {
        ActionManager.DetachPerformer<EndTurnGA>();
        ActionManager.DetachPerformer<StartTurnGA>();
        ActionManager.DetachPerformer<GainMoneyGA>();
        ActionManager.DetachPerformer<GainActionPointsGA>();
        //ActionManager.UnubscribeReaction<EndTurnGA>(EndTurnReaction, ReactionTiming.POST);
    }
    // private void EndTurnReaction(EndTurnGA endTurnGA)
    // {
    //     Debug.Log("turn end detected");
    //     if (HotseatScreenController.instance == null)
    //     {
    //         StartTurnGA startTurnGA = new(currentPlayer);
    //         ActionManager.instance.AddReaction(startTurnGA);
    //     }
    //     else
    //     {
    //         HotseatScreenController.instance.OnTurnEnd();
    //     }
    // }
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
        moneySlider.value = updatePlayer.Money;
    } 
    public void UpdateSciencePointSliders()
    {
        plaSciencePointsSlider.value = players[currentPlayer].sciencePoints;
        oppSciencePointsSlider.value = players[GetNextPlayerId(displayPlayer)].sciencePoints;
    }
    public int GetNextPlayerId(int playerId = -1)
    {
        if (playerId < 0) playerId = currentPlayer;
        return (playerId < players.Count - 1) ? playerId + 1 : 0;
    }

}
