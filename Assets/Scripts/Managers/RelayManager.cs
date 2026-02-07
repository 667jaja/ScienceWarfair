
using System.Collections;
using TMPro;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
public class RelayManager : MonoBehaviour
{
    public static RelayManager instance;
    [SerializeField] private GameObject screenBlock;
    public string joinInput;
    public TextMeshProUGUI joinCode;
    //[SerializeField] private string joinCode;
    //[SerializeField] private string joinInput;

    [SerializeField] private UnityTransport transport;
    [SerializeField] private int maxPlayers;
    [SerializeField] private int minClients;
    [SerializeField] private float maxConnectionTime = 300;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Awake()
    {
        instance = this;
        screenBlock.SetActive(false);
        joinCode.gameObject.SetActive(false);
        // if (SceneLoadManager.gameType == BattleType.OnlineHost || SceneLoadManager.gameType == BattleType.OnlineJoin)
        // {
        //     await Authenticate();
        //     screenBlock.SetActive(true);
        // }
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }


    public async Task CreateGame()
    {
        await Authenticate();
        joinCode.gameObject.SetActive(true);

        screenBlock.SetActive(true);

        Allocation a = await RelayService.Instance.CreateAllocationAsync(maxPlayers, null);
        joinCode.text = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

        transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

        NetworkManager.Singleton.StartHost();
        StartCoroutine(WaitForPlayers());
    }
    public async Task JoinGame()
    {
        await Authenticate();
        screenBlock.SetActive(true);

        screenBlock.SetActive(true);

        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinInput);

        transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes,  a.Key, a.ConnectionData, a.HostConnectionData);

        NetworkManager.Singleton.StartClient();
        StartCoroutine(WaitForPlayers());
    }
    private IEnumerator WaitForPlayers()
    {
        float connectionWait = 0;
        bool connectionSuccess = true;

        while (NetworkManager.Singleton.ConnectedClientsList.Count < minClients)
        {
            connectionWait += Time.deltaTime;
            // if (connectionWait > maxConnectionTime)
            // {
            //     OnlineManager.instance.ConnectionFailure();
            //     connectionSuccess = false;
            //     break;
            // } 
            yield return null;
        }
        if (connectionSuccess)
        {
            StartCoroutine(OnlineManager.instance.PlayerDataSetup());
            screenBlock.SetActive(false);
            Debug.Log("SUCCESS Client Count " + NetworkManager.Singleton.ConnectedClientsList.Count);  
        }

    } 
}
