using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Netcode.Components;
using System.Threading.Tasks;
using System;
using Unity.Mathematics;
using System.Data.Common;
using TMPro;

public class OnlineManager : NetworkBehaviour
{
    public static OnlineManager instance;
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
        UnitManager.instance.PlaceCard(playerId, lane, CardLibraryManager.instance.cardFromCardStruct(cardStruct));
    }
    [Rpc(SendTo.NotServer)]
    public void PlaceUnitGAClientRPC(int playerId, int lane, CardStruct cardStruct)
    {
        UnitManager.instance.PlaceCard(playerId, lane, CardLibraryManager.instance.cardFromCardStruct(cardStruct));
    }

    [Rpc(SendTo.Server)]
    public void PlayActionGAServerRPC(int playerId, CardStruct cardStruct)
    {
        UnitManager.instance.PlayAction(playerId, CardLibraryManager.instance.cardFromCardStruct(cardStruct));
    }
    [Rpc(SendTo.NotServer)]
    public void PlayActionGAClientRPC(int playerId, CardStruct cardStruct)
    {
        UnitManager.instance.PlayAction(playerId, CardLibraryManager.instance.cardFromCardStruct(cardStruct));
    }

    public void StateUpdate()
    {
        if (IsHost)
        {
            PlayerStruct newplayer1 = new PlayerStruct(GameManager.instance.players[0]);
            PlayerStruct newplayer2 = new PlayerStruct(GameManager.instance.players[1]);
            //Debug.Log(newplayer1.name.ArrayToString() +" has "+ newplayer1.hand[0].CardDataBaseId + " and " + newplayer2.name.ArrayToString() +" has "+ newplayer2.hand[0].CardDataBaseId);
            StateUpdateClientRPC(newplayer1, newplayer2);
            GameManager.instance.GlobalUIUpdate();
        }
    }

    [Rpc(SendTo.NotServer)]
    public void StateUpdateClientRPC(PlayerStruct hostPlayerStruct, PlayerStruct clientPlayerStruct)
    {
        Debug.Log("StateUpdateClientRPC Called");

        Player player1 = CardLibraryManager.instance.PlayerFromPlayerStruct(hostPlayerStruct);
        Player player2 = CardLibraryManager.instance.PlayerFromPlayerStruct(clientPlayerStruct);

        // Debug.Log(hostPlayerStruct.name.ArrayToString() +" has "+ hostPlayerStruct.hand[0].CardDataBaseId + " and " + clientPlayerStruct.name.ArrayToString() +" has "+ clientPlayerStruct.hand[0].CardDataBaseId);
        // Debug.Log(player1.name +" has "+ player1.hand[0].cardData.CardDataId + " and " + player2.name +" has "+ player2.hand[0].cardData.CardDataId);

        // //units
        // for (int i = 0; i < 3; i++)
        // {
        //     for (int j = 0; i < 3; i++)
        //     {
        //         if (hostPlayerStruct.units[(i*3) + j].CardDataBaseId > 0) Debug.Log("host has unit " + hostPlayerStruct.units[(i*3) + j].CardDataBaseId+" at " + );
        //     }        
        // }

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
        Debug.Log("player first Card = " + clientPlayerStruct.deck);

        Player hostNewPlayer = new Player(GameManager.instance.playerDatas[SceneLoadManager.selectedPlayerData], 0);
        PlayerStruct newPlayerStruct = new PlayerStruct(hostNewPlayer);

        newPlayerStruct.id = 0;
        clientPlayerStruct.id = 1;

        ClientSetupClientRpc(newPlayerStruct);

        Player player1 = CardLibraryManager.instance.PlayerFromPlayerStruct(newPlayerStruct);
        Player player2 = CardLibraryManager.instance.PlayerFromPlayerStruct(clientPlayerStruct);

        GameManager.instance.players = new() 
        {
            player1,
            player2
        };
        GameManager.instance.displayPlayer = 0;
        GameManager.instance.BeginGame();
    }

    [Rpc(SendTo.NotServer)]
    public void ClientSetupClientRpc(PlayerStruct hostPlayerStruct)
    {
        Debug.Log("ClientSetupClientRpc Called  Host Name is: " + hostPlayerStruct.name.ArrayToString());
        
        Player clientNewPlayer = new Player(GameManager.instance.playerDatas[SceneLoadManager.selectedPlayerData], 1);
        PlayerStruct clientPlayerStruct = new PlayerStruct(clientNewPlayer);

        Player player1 = CardLibraryManager.instance.PlayerFromPlayerStruct(hostPlayerStruct);
        Player player2 = CardLibraryManager.instance.PlayerFromPlayerStruct(clientPlayerStruct);

        GameManager.instance.players = new() 
        {
            player1,
            player2
        };
        GameManager.instance.displayPlayer = 1;
        GameManager.instance.BeginGame();
    }
}
