using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Netcode.Components;
using System.Threading.Tasks;

public class OnlineManager : NetworkBehaviour
{
    public static OnlineManager instance;
    private List<PlayerData> playerDatas;    

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
        yield return null;
        GameManager.instance.BeginGame();
    }
    //     playerDatas = new();
    //     if (IsHost)
    //     {
            
    //         ClientSetupClientRpc();
    //         while (playerDatas.Count < NetworkManager.Singleton.ConnectedClientsList.Count)
    //         {
    //             yield return null;
    //         }
            
    //         playerDatas.Add(GameManager.instance.playerDatas[SceneLoadManager.selectedPlayerData]);
    //         PlayerDatasOverrideServerRpc(playerDatas);
    //         GameManager.instance.playerDatas = playerDatas;
    //         GameManager.instance.BeginGame();
    //     }
    // }
    // [Rpc(SendTo.NotServer)]
    // public void ClientSetupClientRpc()
    // {
    //     HostSetupServerRpc(GameManager.instance.playerDatas[SceneLoadManager.selectedPlayerData]);
    // }
    // [Rpc(SendTo.Server)]
    // public void HostSetupServerRpc(PlayerData newPlayerData)
    // {
    //     playerDatas.Add(newPlayerData);
    // }



    // [Rpc(SendTo.NotServer)]
    // public void PlayerDatasOverrideServerRpc(List<PlayerData> newPlayerDatas)
    // {
    //     GameManager.instance.playerDatas = newPlayerDatas;
    //     GameManager.instance.BeginGame();
    // }
}
