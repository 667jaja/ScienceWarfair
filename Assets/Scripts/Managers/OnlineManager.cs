using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Netcode.Components;
using System.Threading.Tasks;
using System;
using Unity.Mathematics;
using System.Data.Common;

public class OnlineManager : NetworkBehaviour
{
    public static OnlineManager instance;
    private NetworkVariable<PlayerStruct> hostPlayer = new (writePerm: NetworkVariableWritePermission.Server);
    private NetworkVariable<PlayerStruct> clientPlayer = new (writePerm: NetworkVariableWritePermission.Server);


    void Awake()
    {
        instance = this;
    }
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
        GameManager.instance.BeginGame();

    }

    [Rpc(SendTo.NotServer)]
    public void ClientSetupClientRpc(PlayerStruct hostPlayerStruct)
    {
        Debug.Log("ClientSetupClientRpc Called  Host Name is: " + hostPlayer.Value.name);
        
        Player clientNewPlayer = new Player(GameManager.instance.playerDatas[SceneLoadManager.selectedPlayerData], 1);
        PlayerStruct clientPlayerStruct = new PlayerStruct(clientNewPlayer);

        Player player1 = CardLibraryManager.instance.PlayerFromPlayerStruct(hostPlayerStruct);
        Player player2 = CardLibraryManager.instance.PlayerFromPlayerStruct(clientPlayerStruct);

        GameManager.instance.players = new() 
        {
            player1,
            player2
        };
        GameManager.instance.BeginGame();
    }
}
