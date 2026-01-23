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
            players[i].name = "Player" + (i+1);
            players[i].maxMoney = maxMoney;
            players[i].Money = startingMoney + i*turnOrderBonusMoney;
            CardManager.instance.CreateDeck(i);
            CardManager.instance.DrawCards(i, startingCards + i*turnOrderBonusCards);
        }
        
        UpdateMoneyUI(players[0]);
        currentPlayer = 0;
        StartTurnGA startTurnGA = new(currentPlayer);
        startTurnGA.description = "Game start. "+players[0].name+" plays first. money: " + startingMoney + "-" + (startingMoney+turnOrderBonusMoney) + " cards: " + startingCards + "-" + (startingCards + turnOrderBonusCards);
        ActionManager.instance.Perform(startTurnGA);
    }
    public void EndGame()
    {
        players = new();
        ActionManager.instance.ClearAll();
        currentPlayer = 0;

        foreach (Player player in players)
        {
            //for every player
            for (int i = 0; i < UnitManager.instance.columnCount; i++)
            {
                //for every column
                for (int j = 0; j < UnitManager.instance.rowCount; j++)
                {
                    //for every row
                    player.units[i,j].UnsubET();
                }
            }
        }

        StartTurn();
    }
    public void PlayerWon(int winningPlayer)
    {
        if (HotseatScreenController.instance != null)
        {
            HotseatScreenController.instance.YouWin(winningPlayer);
        }
        EndGame();
        // if (HotseatScreenController.instance == null)
        // {
            
        // }
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

        //check win
        int highestPlayerSciencePoints = 0;
        int highestSciencePointsPlayerId = 0;
        foreach (Player player in players)
        {
            if (player.sciencePoints > highestPlayerSciencePoints)
            {
                highestPlayerSciencePoints = player.sciencePoints;
                highestSciencePointsPlayerId = player.id;
            }
        }
        if (highestPlayerSciencePoints > maxSciencePoints)
        {
            PlayerWon(highestSciencePointsPlayerId);
        }
        else
        {
            //start next turn
            if (HotseatScreenController.instance == null)
            {
                StartTurnGA startTurnGA = new(currentPlayer);
                ActionManager.instance.AddReaction(startTurnGA);
            }
            else
            {
                yield return HotseatScreenController.instance.OnTurnEnd();
            }
        }
        // Debug.Log("current player IQ: " + players[currentPlayer].sciencePoints);
        // Debug.Log("next player IQ: " + players[GetNextPlayerId()].sciencePoints);
    }
    public void StartTurn()
    {
        StartTurnGA startTurnGA = new(currentPlayer);
        startTurnGA.description = players[startTurnGA.playerId].name + "'s turn. money: " + players[startTurnGA.playerId].Money + "-" + players[GetNextPlayerId(startTurnGA.playerId)].Money + "  cards: " + (players[startTurnGA.playerId].hand.Count + cardGain) + "-" + (players[GetNextPlayerId(startTurnGA.playerId)].hand.Count + cardGain);// + " SP: "  + players[startTurnGA.playerId].sciencePoints + "-" + players[GetNextPlayerId(startTurnGA.playerId)].sciencePoints;
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
    private IEnumerator GainSciencePointsPerformer(GainSciencePointsGA gainSciencePointsGA)
    {
        yield return LaneManager.instance.AddIqVisual(gainSciencePointsGA.playerId, gainSciencePointsGA.gainCount);
        players[gainSciencePointsGA.playerId].sciencePoints += gainSciencePointsGA.gainCount;
        UpdateSciencePointSliders();
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
        ActionManager.AttachPerformer<GainSciencePointsGA>(GainSciencePointsPerformer);
        //ActionManager.SubscribeReaction<EndTurnGA>(EndTurnReaction, ReactionTiming.POST);

    }
    private void OnDisable()
    {
        ActionManager.DetachPerformer<EndTurnGA>();
        ActionManager.DetachPerformer<StartTurnGA>();
        ActionManager.DetachPerformer<GainMoneyGA>();
        ActionManager.DetachPerformer<GainActionPointsGA>();
        ActionManager.DetachPerformer<GainSciencePointsGA>();
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
