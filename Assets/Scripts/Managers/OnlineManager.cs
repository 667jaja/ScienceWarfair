using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Threading.Tasks;

public class OnlineManager : NetworkBehaviour
{
    public static OnlineManager instance;
    [SerializeField] private float maxStateUpdateWait = 5;
    private bool hasUpdated;
    // private NetworkVariable<PlayerStruct> hostPlayer = new (writePerm: NetworkVariableWritePermission.Server);
    // private NetworkVariable<PlayerStruct> clientPlayer = new (writePerm: NetworkVariableWritePermission.Server);

    void Awake()
    {
        instance = this;
    }
    
    public void InputGameAction(GameAction gameAction)
    {
        if (gameAction.inputPlayerId == GameManager.instance.currentPlayer)
        {
            if (gameAction is EndTurnGA endTurn) 
            { 
                if (IsHost) EndTurnGAClientRPC(endTurn.playerId); 
                if (!IsHost) EndTurnGAServerRPC(endTurn.playerId); 
            }
            if (gameAction is PlaceUnitGA placeUnit)
            { 
                if (IsHost) PlaceUnitGAClientRPC(placeUnit.playerId, placeUnit.lane, new CardStruct(placeUnit.playedCard)); 
                if (!IsHost) PlaceUnitGAServerRPC(placeUnit.playerId, placeUnit.lane, new CardStruct(placeUnit.playedCard)); 
            }
            if (gameAction is PlayActionGA playAction)
            { 
                if (IsHost) PlayActionGAClientRPC(playAction.playerId, new CardStruct(playAction.playedCard)); 
                if (!IsHost) PlayActionGAServerRPC(playAction.playerId, new CardStruct(playAction.playedCard)); 
            }   
        }

        // if (gameAction is SelectUnitsGA selectUnits) 
        // { 
        //     if (IsHost) EndTurnGAClientRPC(selectUnits.playerId); 
        //     if (!IsHost) EndTurnGAServerRPC(selectUnits.playerId); 
        // }
        // if (gameAction is SelectLanesGA selectLanes) 
        // { 
        //     if (IsHost) EndTurnGAClientRPC(selectLanes.playerId); 
        //     if (!IsHost) EndTurnGAServerRPC(selectLanes.playerId); 
        // }
    }
    public void InputSelection()
    {
        //prep data
        //hand
        CardStruct[] selectedHand = new CardStruct[SelectionManager.instance.selectedHand.Count];
        for (int i = 0; i < selectedHand.Length; i++)
        {
            selectedHand[i] = new CardStruct(SelectionManager.instance.selectedHand[i]);
        }
        //discard
        int[] selectedDiscard = new int[SelectionManager.instance.selectedDiscard.Count];
        for (int i = 0; i < selectedDiscard.Length; i++)
        {
            selectedDiscard[i] = SelectionManager.instance.selectedDiscard[i].cardData.CardDataId;
        }
        //units
        Vector3Int[] selectedBoard = new Vector3Int[SelectionManager.instance.selectedBoard.Count];
        for (int i = 0; i < selectedBoard.Length; i++)
        {
            selectedBoard[i] = SelectionManager.instance.selectedBoard[i];
        }
        //lanes
        Vector2Int[] selectedLanes = new Vector2Int[SelectionManager.instance.selectedLanes.Count];
        for (int i = 0; i < selectedLanes.Length; i++)
        {
            selectedLanes[i] = SelectionManager.instance.selectedLanes[i];
        }

        //send rpc
        if (IsHost) InputSelectionClientRPC(GameManager.instance.displayPlayer, selectedHand, selectedDiscard, selectedBoard, selectedLanes);
        if (!IsHost) InputSelectionServerRPC(GameManager.instance.displayPlayer, selectedHand, selectedDiscard, selectedBoard, selectedLanes);
    }
    [Rpc(SendTo.Server)]
    public void InputSelectionServerRPC(int selectingPlayerId, CardStruct[] selectedHand, int[] selectedDiscard, Vector3Int[] selectedBoard, Vector2Int[] selectedLanes)
    {
        Debug.Log("InputSelectionServerRPC Recieved");
        SelectionManager.instance.selectedHand = new();
        SelectionManager.instance.selectedDiscard = new();
        SelectionManager.instance.selectedBoard = new();
        SelectionManager.instance.selectedLanes = new();

        foreach (CardStruct cardStruct in selectedHand)
        {
            SelectionManager.instance.selectedHand.Add(CardManager.instance.FindCardInHand(selectingPlayerId, cardStruct.cardInstanceId, cardStruct.CardDataBaseId));
        }
        // foreach (int cardDataId in selectedDiscard)
        // {
        //      SelectionManager.instance.selectedHand.Add(CardManager.instance.FindCardInDiscard(selectingPlayerId, cardDataId));
        // }
        foreach (Vector3Int vector in selectedBoard)
        {
            SelectionManager.instance.selectedBoard.Add(vector);
        }
        foreach (Vector2Int vector in selectedLanes)
        {
            SelectionManager.instance.selectedLanes.Add(vector);
        }
    }
    [Rpc(SendTo.NotServer)]
    public void InputSelectionClientRPC(int selectingPlayerId, CardStruct[] selectedHand, int[] selectedDiscard, Vector3Int[] selectedBoard, Vector2Int[] selectedLanes)
    {
        Debug.Log("InputSelectionClientRPC Recieved");
        SelectionManager.instance.selectedHand = new();
        SelectionManager.instance.selectedDiscard = new();
        SelectionManager.instance.selectedBoard = new();
        SelectionManager.instance.selectedLanes = new();

        foreach (CardStruct cardStruct in selectedHand)
        {
            SelectionManager.instance.selectedHand.Add(CardManager.instance.FindCardInHand(selectingPlayerId, cardStruct.cardInstanceId, cardStruct.CardDataBaseId));
        }
        // foreach (int cardDataId in selectedDiscard)
        // {
        //      SelectionManager.instance.selectedHand.Add(CardManager.instance.FindCardInDiscard(selectingPlayerId, cardDataId));
        // }
        foreach (Vector3Int vector in selectedBoard)
        {
            SelectionManager.instance.selectedBoard.Add(vector);
        }
        foreach (Vector2Int vector in selectedLanes)
        {
            SelectionManager.instance.selectedLanes.Add(vector);
        }
    }

    [Rpc(SendTo.Server)]
    public void EndTurnGAServerRPC(int playerId)
    {
        GameManager.instance.EndTurn(playerId);
    }
    [Rpc(SendTo.NotServer)]
    public void EndTurnGAClientRPC(int playerId)
    {
        GameManager.instance.EndTurn(playerId);
    }

    [Rpc(SendTo.Server)]
    public void PlaceUnitGAServerRPC(int playerId, int lane, CardStruct cardStruct)
    {
        UnitManager.instance.PlaceCard(playerId, lane, CardLibraryManager.instance.CardFromCardStruct(cardStruct));
    }
    [Rpc(SendTo.NotServer)]
    public void PlaceUnitGAClientRPC(int playerId, int lane, CardStruct cardStruct)
    {
        UnitManager.instance.PlaceCard(playerId, lane, CardLibraryManager.instance.CardFromCardStruct(cardStruct));
    }

    [Rpc(SendTo.Server)]
    public void PlayActionGAServerRPC(int playerId, CardStruct cardStruct)
    {
        UnitManager.instance.PlayAction(playerId, CardLibraryManager.instance.CardFromCardStruct(cardStruct));
    }
    [Rpc(SendTo.NotServer)]
    public void PlayActionGAClientRPC(int playerId, CardStruct cardStruct)
    {
        UnitManager.instance.PlayAction(playerId, CardLibraryManager.instance.CardFromCardStruct(cardStruct));
    }

    public IEnumerator StateUpdate()
    {
        if (IsHost)
        {
            PlayerStruct newplayer1 = new PlayerStruct(GameManager.instance.players[0]);
            PlayerStruct newplayer2 = new PlayerStruct(GameManager.instance.players[1]);
            //Debug.Log(newplayer1.name.ArrayToString() +" has "+ newplayer1.hand[0].CardDataBaseId + " and " + newplayer2.name.ArrayToString() +" has "+ newplayer2.hand[0].CardDataBaseId);
            StateUpdateClientRPC(newplayer1, newplayer2);
        }
        else
        {
            float timer = 0;
            while (timer < maxStateUpdateWait && !hasUpdated)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            hasUpdated = false;
        }
    }


    [Rpc(SendTo.NotServer)]
    public void StateUpdateClientRPC(PlayerStruct hostPlayerStruct, PlayerStruct clientPlayerStruct)
    {
        Debug.Log("StateUpdateClientRPC Called");
        hasUpdated = true;
        GameManager.instance.UnSubAllEts();

        Player player1 = CardLibraryManager.instance.PlayerFromPlayerStruct(hostPlayerStruct);
        Player player2 = CardLibraryManager.instance.PlayerFromPlayerStruct(clientPlayerStruct);

        GameManager.instance.players = new() 
        {
            player1,
            player2
        };
        GameManager.instance.GlobalUIUpdate();
    }

    //setup
    public void ConnectionFailure()
    {
        SceneLoadManager.instance.LoadMainMenu();
    }
    public void ShutDownServer() 
    {
        if (IsHost)
        {
            NetworkManager.Singleton.Shutdown();   
        }
    }

    public IEnumerator PlayerDataSetup()
    {
        //yield return null;
        yield return new WaitUntil(() => IsSpawned);

        if (!IsHost)
        {
            Player clientNewPlayer = new Player(GameManager.instance.playerDatas[SceneLoadManager.selectedPlayerData], 1);
            PlayerStruct newPlayerStruct = new PlayerStruct(clientNewPlayer);

            Debug.Log("Sending HostSetupServerRpc   Client Name: " + newPlayerStruct.name);
            HostSetupServerRpc(newPlayerStruct);
        }
    }

    [Rpc(SendTo.Server)]
    public void HostSetupServerRpc(PlayerStruct clientPlayerStruct)
    {
        //Debug.Log("player first Card = " + clientPlayerStruct.deck);
        // int seed = Random.Range(0, 1000);
        // Random.InitState(seed);

        //create players
        Player player1 = new Player(GameManager.instance.playerDatas[SceneLoadManager.selectedPlayerData], 0);
        Player player2 = CardLibraryManager.instance.PlayerFromPlayerStruct(clientPlayerStruct);
        player2.id = 1;

        player1.deck = GameManager.instance.CreateDeck(player1);
        player2.deck = GameManager.instance.CreateDeck(player2);

        player1.opener = GameManager.instance.CreateOpener(player1);
        player2.opener = GameManager.instance.CreateOpener(player2);

        //send to client
        PlayerStruct newHostStruct = new PlayerStruct(player1);
        PlayerStruct newClientStruct = new PlayerStruct(player2);
        ClientSetupClientRpc(newHostStruct, newClientStruct);

        //start game
        GameManager.instance.players = new() 
        {
            player1,
            player2
        };
        GameManager.instance.displayPlayer = 0;
        GameManager.instance.BeginGame();
    }

    [Rpc(SendTo.NotServer)]
    public void ClientSetupClientRpc(PlayerStruct hostPlayerStruct, PlayerStruct clientPlayerStruct)
    {
        Debug.Log("ClientSetupClientRpc Called  Host Name is: " + hostPlayerStruct.name.ArrayToString());
        //Random.InitState(seed);

        //recontruct players
        Player player1 = CardLibraryManager.instance.PlayerFromPlayerStruct(hostPlayerStruct);
        Player player2 = CardLibraryManager.instance.PlayerFromPlayerStruct(clientPlayerStruct);

        //start game
        GameManager.instance.players = new() 
        {
            player1,
            player2
        };
        GameManager.instance.displayPlayer = 1;
        GameManager.instance.BeginGame();
    }
}
