using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

    //manages overarching elements of a game such as players, loss, victory, turn start, turn end
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    //players
    [SerializeField] private int playerCount = 2;
    public List<PlayerData> playerDatas;
    public List<Player> players = new List<Player>();
    public int currentPlayer;
    public int displayPlayer;

    //ui
    [SerializeField] private GameObject ifCurrentPlayerIsNotDisplay;

    //money
    [SerializeField] private Slider moneySlider;
    [SerializeField] private Slider moneySliderEnemy;
    [SerializeField] private Animator moneyAnimator;
    [SerializeField] private Animator moneyAnimatorEnemy;
    [SerializeField] private string MoneyIncreasingAnimbool;
    [SerializeField] private string MoneyDecreasingAnimbool;
    [SerializeField] private float moneySpeed;

    //science points
    public int maxSciencePoints;

    public Slider plaSciencePointsSlider;
    public Slider oppSciencePointsSlider;
    public TMP_Text plaSciencePointNeeded;
    public TMP_Text plaCurrentSciencePoints;
    public TMP_Text oppSciencePointNeeded;
    public TMP_Text oppCurrentSciencePoints;


    public Transform plaSciencePointsWarning;
    public Transform oppSciencePointsWarning;

    //action points
    public TMP_Text plaActionPointsCount;
    public GameObject plaActionPointObject;


    //money&Cards
    public List<CardData> defaultDeck;
    [SerializeField] private List<int> secretCardIds;
    // public List<CardData> defaultOpener;
    [SerializeField] private int startingMoney;
    [SerializeField] private int turnOrderBonusMoney = 1;
    [SerializeField] private int startingCards;
    [SerializeField] private int turnOrderBonusCards = 1;
    [SerializeField] private int moneyGain = 2;
    [SerializeField] private int cardGain = 2;
    [SerializeField] private int maxMoney;
    private int deckDataSizeMin = 20;

    [SerializeField] private float endOfTurnWait = 0.2f;

    //random
    private float randomVal = -1;
    private float savedRandomVal = -1;
    private List<List<int>> randomRangeResults = new();


    //public int perspective player
    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        ifCurrentPlayerIsNotDisplay.SetActive(currentPlayer != displayPlayer);
    }
    public IEnumerator BeginGame()
    {
        //setup only run on local games
        if (RelayManager.instance == null)
        {
            players = new();
            for (int i = 0; i < playerCount; i++)
            {
            // - for each player

                //create player from player data (scriptable object)
                players.Add(new Player(playerDatas[i], i));

                //setup opener
                players[i].opener = CreateOpener(players[i]);
            }
        }

        //general setup
        for (int i = 0; i < playerCount; i++)
        {
        // - for each player
            //replace invalid name
            if (players[i].name == null || players[i].name.Length < 1) players[i].name = "Player" + (i+1);

            //manage money
            players[i].maxMoney = maxMoney;
            players[i].Money = startingMoney + i*turnOrderBonusMoney;
            UpdateMoneyUI(true);

            //replace raw deck
            if (players[i].rawDeck == null || players[i].rawDeck.Count < deckDataSizeMin) players[i].rawDeck = defaultDeck.ToList();

            //shuffle raw deck into actual deck
            yield return CardManager.instance.AwaitCreateDeck(players[i].rawDeck.ToList());
            players[i].deck = CardManager.instance.CreateDeck();

            //spawn opener units
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

        //draw
        for (int i = 0; i < playerCount; i++)
        {
        // - for each player
            //draw starting hand
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

        //p1 goes first (change to random later)
        currentPlayer = 0;
        //if local display currentPlayer
        if (RelayManager.instance == null) displayPlayer = currentPlayer;

        //queue start of currentPlayer's turn 
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
    }
    public void ExitGame()
    {
        EndGameCleanup();
        SceneLoadManager.instance.LoadMainMenu();
    }
    public void PlayerWon(int winningPlayer)
    {
        //trigger the you win screen for local
        if (HotseatScreenManager.instance != null)
        {
            HotseatScreenManager.instance.YouWin(winningPlayer);
        }

        //cleanup
        EndGameCleanup();
        
        // for online load menu
        if (HotseatScreenManager.instance == null)
        {
            SceneLoadManager.instance.LoadMainMenu();
        }
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
    public IEnumerator AwaitCreateDeckPlayer(Player player)
    {
        if (player.rawDeck == null || player.rawDeck.Count < deckDataSizeMin) player.rawDeck = defaultDeck;
        yield return CardManager.instance.AwaitCreateDeck(player.rawDeck.ToList());
    }
    public List<CardData> CreateOpener(Player player)
    {
        //assigns positions to the units in opener
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
        EndTurn(displayPlayer);
    }
    public void EndTurn(int playerId)
    {
        //if called by the current player
        if (playerId == currentPlayer)
        {
            //queue end of turn
            EndTurnGA endTurnGA = new(playerId);
            ActionManager.instance.Perform(endTurnGA);
        }
    }
    public void EndTurnDraw(EndTurnGA endTurnGA)
    {
        ActionManager.instance.AddReaction(new DrawCardsGA(currentPlayer, cardGain));
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
        players[currentPlayer].SciencePoints += UnitManager.instance.CountPlayerIQ(currentPlayer);
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
            if (player.SciencePoints > highestPlayerSciencePoints)
            {
                highestPlayerSciencePoints = player.SciencePoints;
                highestSciencePointsPlayerId = player.id;
            }
        }
        if (highestPlayerSciencePoints >= maxSciencePoints)
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
        players[gainSciencePointsGA.playerId].SciencePoints += gainSciencePointsGA.gainCount;
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
        ActionManager.SubscribeReaction<EndTurnGA>(EndTurnDraw, ReactionTiming.PRE);
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
        plaSciencePointsSlider.value = players[displayPlayer].SciencePoints;
        oppSciencePointsSlider.value = players[GetNextPlayerId(displayPlayer)].SciencePoints;

        plaSciencePointNeeded.text = (maxSciencePoints - players[displayPlayer].SciencePoints).ToString();
        plaCurrentSciencePoints.text = players[displayPlayer].SciencePoints.ToString();

        oppSciencePointNeeded.text = (maxSciencePoints - players[GetNextPlayerId(displayPlayer)].SciencePoints).ToString();
        oppCurrentSciencePoints.text = players[GetNextPlayerId(displayPlayer)].SciencePoints.ToString();


        plaSciencePointsWarning.gameObject.SetActive(UnitManager.instance.CountPlayerIQ(displayPlayer) + players[displayPlayer].SciencePoints >= maxSciencePoints);
        oppSciencePointsWarning.gameObject.SetActive(UnitManager.instance.CountPlayerIQ(GetNextPlayerId(displayPlayer)) + players[GetNextPlayerId(displayPlayer)].SciencePoints >= maxSciencePoints);

        LaneManager.instance.UpdateLaneVisuals();
    }

    //get
    public int GetNextPlayerId(int playerId = -1)
    {
        if (playerId < 0) playerId = currentPlayer;
        return (playerId < players.Count - 1) ? playerId + 1 : 0;
    }

    //when other script needs random variables they call this with: yield return AwaitNewRandomRange(ranges);
    public IEnumerator AwaitNewRandomRange(List<Vector2Int> ranges)
    {
        // Host generates and sends
        if (RelayManager.instance == null || OnlineManager.instance.IsHost)
        {
            Debug.Log("is host or offline, generating randRanges");
            // Generate values
            List<int> values = new();
            foreach (Vector2Int range in ranges)
                values.Add(Random.Range(range.x,range.y));

            // Rpc send to Client
            if (RelayManager.instance != null) OnlineManager.instance.InputRandom(values);
            randomRangeResults = new List<List<int>> {values};
        }

        // await random Ints set
        if (RelayManager.instance != null && !OnlineManager.instance.IsHost) Debug.Log("started randRange wait phase. " + randomRangeResults.Count + " value set(s) already saved");
        else Debug.Log("started randRange wait phase");

        float timer = 0;
        float timerLoopCount = 0;
        float timerAlertFreq = 3;
        while (randomRangeResults.Count == 0)
        {
            timer += Time.deltaTime;
            if (timer > timerAlertFreq)
            {
                timer = 0;
                timerLoopCount++;
                Debug.LogError("unresolved randRange while loop from " + (timerLoopCount*timerAlertFreq) + "seconds ago");
            }
            yield return null;
        }
        Debug.Log("ended randRange wait phase " + randomRangeResults[0].Count + " values Generated" + (randomRangeResults[0].Count>1? ", first is: " + randomRangeResults[0][0] : "."));

    }
    //when yield return AwaitNewRandomRange is done other scripts fet their results here
    public List<int> GetRandomRangeVal()
    {
        List<int> returnVal = randomRangeResults[0];
        randomRangeResults.RemoveAt(0);
        return returnVal;
    }
    public void AddRandomRangeVal(List<int> setVal)
    {
        randomRangeResults.Add(setVal);
    }
  }

    // public IEnumerator AwaitNewRandomVal()
    // {
    //     //if host send random val
    //     if (RelayManager.instance == null || OnlineManager.instance.IsHost)
    //     {
    //         randomVal = Random.value;
    //         if (RelayManager.instance != null) OnlineManager.instance.InputRandom(null, randomVal);
    //     }
        
    //     Debug.Log("started randVal wait phase");
    //     //await randomVal set
    //     while (randomVal < 0)
    //     {
    //         yield return null;
    //     }
    //     Debug.Log("ended randVal wait phase Value Generated: " + randomVal);

    //     //save value for use
    //     savedRandomVal = randomVal;
    //     randomVal = -1;
    // }
    // public float GetRandVal()
    // {
    //     return savedRandomVal;
    // }
    // public void SetRandVal(float setVal)
    // {
    //     randomVal = setVal;
    // }