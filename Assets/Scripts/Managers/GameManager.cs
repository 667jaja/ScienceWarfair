using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    //manages overarching elements of a game such as players, loss, victory, turn start, turn end
    public static GameManager instance;
    [SerializeField] private int playerCount = 2;
    public int maxSciencePoints;
    
    //players
    public List<PlayerData> playerDatas;
    public List<Player> players = new List<Player>();
    public int currentPlayer;
    public int displayPlayer;

    //ui
    //money
    [SerializeField] private Slider moneySlider;
    [SerializeField] private Slider moneySliderEnemy;
    [SerializeField] private Animator moneyAnimator;
    [SerializeField] private Animator moneyAnimatorEnemy;
    [SerializeField] private string MoneyIncreasingAnimbool;
    [SerializeField] private string MoneyDecreasingAnimbool;
    [SerializeField] private float moneySpeed;

    //science points
    public Slider plaSciencePointsSlider;
    public Slider oppSciencePointsSlider;
    public TMP_Text plaSciencePointNeeded;
    public TMP_Text plaCurrentSciencePoints;
    public TMP_Text oppSciencePointNeeded;
    public TMP_Text oppCurrentSciencePoints;


    public Transform plaSciencePointsWarning;
    public Transform oppSciencePointsWarning;

    public TMP_Text plaActionPointsCount;
    public GameObject plaActionPointObject;

    [SerializeField] private GameObject ifCurrentPlayerIsNotDisplay;
    [SerializeField] private List<int> secretCardIds;

    //money&Cards
    public List<CardData> defaultDeck;
    // public List<CardData> defaultOpener;
    [SerializeField] private int startingMoney;
    [SerializeField] private int turnOrderBonusMoney = 1;
    [SerializeField] private int startingCards;
    [SerializeField] private int turnOrderBonusCards = 1;
    [SerializeField] private int moneyGain = 2;
    [SerializeField] private int cardGain = 2;
    [SerializeField] private int maxMoney;
    public int deckDataSizeMin = 20;

    [SerializeField] private float endOfTurnWait = 0.2f;

    //public int perspective player
    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        ifCurrentPlayerIsNotDisplay.SetActive(currentPlayer != displayPlayer);
    }
    public void BeginGame()
    {
        //local game only setup
        if (RelayManager.instance == null)
        {
            players = new();
            for (int i = 0; i < playerCount; i++)
            {
                players.Add(new Player(playerDatas[i], i));
                if (players[i].rawDeck == null || players[i].rawDeck.Count < deckDataSizeMin) players[i].rawDeck = defaultDeck;
                players[i].deck = CardManager.instance.CreateDeck(players[i].rawDeck);
                players[i].opener = CreateOpener(players[i]);
            }
        }

        //standard setup
        for (int i = 0; i < playerCount; i++)
        {
            if (players[i].name == null || players[i].name.Length < 1) players[i].name = "Player" + (i+1);
            players[i].maxMoney = maxMoney;
            players[i].Money = startingMoney + i*turnOrderBonusMoney;
            UpdateMoneyUI(true);
            int c = 0;
            foreach (CardData item in players[i].opener)
            {
                if (c == 3) c = 0;
                if (item != null)
                {
                    CreateUnitGA createUnitGA = new(players[i].id, new Vector2Int(c,0), new Card(item));
                    createUnitGA.description = players[i].name + " plays opening unit " + item.title;
                    ActionManager.instance.Perform(createUnitGA);
                }
                c++;
            }
        }
        for (int i = 0; i < playerCount; i++)
        {
            CardManager.instance.DrawCards(i, startingCards + i*turnOrderBonusCards);
        }

        //cheats
        for (int i = 0; i < playerCount; i++)
        {
            if (players[i].name.Contains("Jackal667"))
            {
                players[i].name = "Jackal";
                CardManager.instance.AddtoHand(i, CardLibraryManager.instance.GetCardDataById(secretCardIds[0]));
            }
            if (players[i].name.Contains("Samuel88"))
            {
                players[i].name = "Samuel";
                CardManager.instance.AddtoHand(i, CardLibraryManager.instance.GetCardDataById(secretCardIds[2]));
            }
            if (players[i].name.Contains("Miles1"))
            {
                players[i].name = "Miles";

                for (int c = 0; c < 3; c++)
                {
                    CreateUnitGA createUnitGA = new(players[i].id, new Vector2Int(c,0), new Card(CardLibraryManager.instance.GetCardDataById(secretCardIds[1])));
                    createUnitGA.description = "Miles Cheats";
                    ActionManager.instance.Perform(createUnitGA);
                }
            }
        }

        currentPlayer = 0;
        if (RelayManager.instance == null) displayPlayer = currentPlayer;
        StartTurnGA startTurnGA = new(currentPlayer);
        startTurnGA.description = "Game start. "+players[0].name+" plays first. money: " + startingMoney + "-" + (startingMoney+turnOrderBonusMoney) + " cards: " + startingCards + "-" + (startingCards + turnOrderBonusCards);
        ActionManager.instance.Perform(startTurnGA);
    }
    public void EndGameCleanup()
    {
        players = new();
        ActionManager.instance.ClearAll();
        currentPlayer = 0;
        UnSubAllEts();

        if (RelayManager.instance != null)
        {
            OnlineManager.instance.ShutDownServer();
        }
        //StartTurn();
    }
    public void ExitGame()
    {
        EndGameCleanup();
        SceneLoadManager.instance.LoadMainMenu();
    }
    public void PlayerWon(int winningPlayer)
    {
        if (HotseatScreenManager.instance != null)
        {
            HotseatScreenManager.instance.YouWin(winningPlayer);
        }
        EndGameCleanup();

        if (HotseatScreenManager.instance == null)
        {
            SceneLoadManager.instance.LoadMainMenu();
        }
        // if (HotseatScreenManager.instance == null)
        // {
            
        // }
    }
    public void UnSubAllEts()
    {
        foreach (Player player in players)
        {
            //for every player
            for (int i = 0; i < UnitManager.instance.columnCount; i++)
            {
                //for every column
                for (int j = 0; j < UnitManager.instance.rowCount; j++)
                {
                    //for every row
                    if (player.units[i,j] != null) player.units[i,j].UnsubET();
                }
            }
        }
    }
    public List<CardData> CreateDeck(Player player)
    {
        if (player.rawDeck == null || player.rawDeck.Count < deckDataSizeMin) player.rawDeck = defaultDeck;
        return CardManager.instance.CreateDeck(player.rawDeck);
    }
    public List<CardData> CreateOpener(Player player)
    {
        // if (player.opener == null || player.opener.Count < 1) player.opener = defaultOpener;
        CardData[] newOpener = new CardData[9];
        List<int> validPositions = new List<int>() {0,1,2,3,4,5,6,7,8};
        foreach (CardData data in player.opener)
        {
            int rand = Random.Range(0, validPositions.Count);
            rand = validPositions[rand];
            validPositions.Remove(rand);

            newOpener[rand] = data;
        }
        return newOpener.ToList();
    }
    public void TurnEndButton()
    {
        //if (ActionManager.instance.isPerforming) return;
        EndTurn(displayPlayer);
    }
    public void EndTurn(int playerId)
    {
        if (playerId == currentPlayer)
        {
            CardManager.instance.CurrentPlayerDrawCards(cardGain);

            EndTurnGA endTurnGA = new(playerId);
            ActionManager.instance.Perform(endTurnGA);
        }
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
        UpdateSciencePointsUI();

        //stall
        yield return new WaitForSeconds(endOfTurnWait);

        //currentplayer
        currentPlayer = GetNextPlayerId();
        if (RelayManager.instance == null) displayPlayer = currentPlayer;

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
            if (HotseatScreenManager.instance == null)
            {
                StartTurnGA startTurnGA = new(currentPlayer);
                startTurnGA.description = players[startTurnGA.playerId].name + "'s turn. money: " + players[startTurnGA.playerId].Money + "-" + players[GetNextPlayerId(startTurnGA.playerId)].Money + "  cards: " + (players[startTurnGA.playerId].hand.Count + cardGain) + "-" + (players[GetNextPlayerId(startTurnGA.playerId)].hand.Count + cardGain);
                ActionManager.instance.AddReaction(startTurnGA);
            }
            else
            {
                yield return HotseatScreenManager.instance.OnTurnEnd();
            }
        }
        if (RelayManager.instance != null)
        {
            yield return OnlineManager.instance.StateUpdate();
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
        yield return SetMoneyVisual(true, moneySlider.value, players[displayPlayer].Money);
        UpdateMoneyUI();
    }
    private IEnumerator GainSciencePointsPerformer(GainSciencePointsGA gainSciencePointsGA)
    {
        yield return LaneManager.instance.AddIqVisual(gainSciencePointsGA.playerId, gainSciencePointsGA.gainCount);
        players[gainSciencePointsGA.playerId].sciencePoints += gainSciencePointsGA.gainCount;
        UpdateSciencePointsUI();
    }
    private IEnumerator GainActionPointsPerformer(GainActionPointsGA gainActionPointsGA)
    {
        players[gainActionPointsGA.playerId].actionPoints += gainActionPointsGA.gainCount;
        UpdateActionPointUI();
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

    //ui
    public IEnumerator SetMoneyVisual(bool currentPlayer, float oldMoneyVal, float newMoneyVal)
    {
        if (oldMoneyVal != newMoneyVal)
        {        
            bool isIncreasing = oldMoneyVal<newMoneyVal;
            if (currentPlayer)
            {
                moneyAnimator.SetBool(MoneyIncreasingAnimbool, isIncreasing);
                moneyAnimator.SetBool(MoneyDecreasingAnimbool, !isIncreasing);
            } 
            else 
            {
                moneyAnimatorEnemy.SetBool(MoneyIncreasingAnimbool, isIncreasing);
                moneyAnimatorEnemy.SetBool(MoneyDecreasingAnimbool, !isIncreasing);
            }

            float timer = 0;
            float slope =  isIncreasing? moneySpeed: -moneySpeed;
            while (timer <  Mathf.Abs((newMoneyVal-oldMoneyVal)/moneySpeed))
            {
                timer += Time.deltaTime;
                if (currentPlayer) moneySlider.value = oldMoneyVal + slope*timer;
                else moneySliderEnemy.value = oldMoneyVal + slope*timer;
                yield return null;
            }
        }

        if (currentPlayer) moneySlider.value = newMoneyVal;
        else moneySliderEnemy.value = newMoneyVal;
        if (currentPlayer)
        {
            moneyAnimator.SetBool(MoneyIncreasingAnimbool, false);
            moneyAnimator.SetBool(MoneyDecreasingAnimbool, false);
        } 
        else 
        {
            moneyAnimatorEnemy.SetBool(MoneyIncreasingAnimbool, false);
            moneyAnimatorEnemy.SetBool(MoneyDecreasingAnimbool, false);
        }
    }

    public void GlobalUIUpdate()
    {
        CardManager.instance.UpdateDeckUI();
        CardManager.instance.UpdateHandUI();
        UnitManager.instance.UpdateUnitUI();
        UpdateActionPointUI();
        UpdateMoneyUI(true);
        UpdateSciencePointsUI();
        LaneManager.instance.UpdateLaneVisuals();
    }
    public void UpdateActionPointUI()
    {
        plaActionPointObject.SetActive(players[displayPlayer].actionPoints > 0);
        plaActionPointsCount.gameObject.SetActive(players[displayPlayer].actionPoints > 1);

        plaActionPointsCount.text = players[displayPlayer].actionPoints.ToString();
    }
    public void UpdateMoneyUI(bool skipAnim = false)
    {
        if (skipAnim)
        {
            moneySlider.value = players[displayPlayer].Money;
            moneySliderEnemy.value = players[GetNextPlayerId(displayPlayer)].Money;

            moneyAnimator.SetBool(MoneyIncreasingAnimbool, false);
            moneyAnimator.SetBool(MoneyDecreasingAnimbool, false);
            moneyAnimatorEnemy.SetBool(MoneyIncreasingAnimbool, false);
            moneyAnimatorEnemy.SetBool(MoneyDecreasingAnimbool, false);
        }
        else
        {
            StartCoroutine(SetMoneyVisual(true, moneySlider.value, players[displayPlayer].Money));
            StartCoroutine(SetMoneyVisual(false, moneySliderEnemy.value, players[GetNextPlayerId(displayPlayer)].Money));
        }

    } 
    public void UpdateSciencePointsUI()
    {
        plaSciencePointsSlider.value = players[displayPlayer].sciencePoints;
        oppSciencePointsSlider.value = players[GetNextPlayerId(displayPlayer)].sciencePoints;

        plaSciencePointNeeded.text = (maxSciencePoints - players[displayPlayer].sciencePoints).ToString();
        plaCurrentSciencePoints.text = players[displayPlayer].sciencePoints.ToString();

        oppSciencePointNeeded.text = (maxSciencePoints - players[GetNextPlayerId(displayPlayer)].sciencePoints).ToString();
        oppCurrentSciencePoints.text = players[GetNextPlayerId(displayPlayer)].sciencePoints.ToString();


        plaSciencePointsWarning.gameObject.SetActive(UnitManager.instance.CountPlayerIQ(displayPlayer) + players[displayPlayer].sciencePoints >= maxSciencePoints);
        oppSciencePointsWarning.gameObject.SetActive(UnitManager.instance.CountPlayerIQ(GetNextPlayerId(displayPlayer)) + players[GetNextPlayerId(displayPlayer)].sciencePoints >= maxSciencePoints);

        LaneManager.instance.UpdateLaneVisuals();
    }

    //get
    public int GetNextPlayerId(int playerId = -1)
    {
        if (playerId < 0) playerId = currentPlayer;
        return (playerId < players.Count - 1) ? playerId + 1 : 0;
    }

}
