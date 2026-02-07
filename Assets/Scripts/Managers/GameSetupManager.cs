using System.Threading.Tasks;
using UnityEngine;

public class GameSetupManager : MonoBehaviour
{
    public static GameSetupManager instance;
    [SerializeField] private GameObject whileSceneLoading;

    void Awake()
    {
        whileSceneLoading.SetActive(true);

        instance = this;
    }
    public async void Start()
    {
        if (SceneLoadManager.gameType == BattleType.HotSeat)
        {
            //LOCAL

            HotseatScreenManager.instance.enabled = true;

            if (RelayManager.instance != null)
            {
                RelayManager.instance.enabled = false;
                RelayManager.instance = null;
            }

            HotseatScreenManager.instance.Initiate();
            whileSceneLoading.SetActive(false);
        }
        else if (SceneLoadManager.gameType == BattleType.OnlineHost)
        {
            //HOST

            if (HotseatScreenManager.instance != null)
            {
                HotseatScreenManager.instance.enabled = false;
                HotseatScreenManager.instance = null;
            }

            RelayManager.instance.enabled = true;
            await RelayManager.instance.CreateGame();

            //GameManager.instance.BeginGame();
            whileSceneLoading.SetActive(false);
        }
        else if (SceneLoadManager.gameType == BattleType.OnlineJoin)
        {
            //JOIN

            if (HotseatScreenManager.instance != null)
            {
                HotseatScreenManager.instance.enabled = false;
                HotseatScreenManager.instance = null;
            }

            RelayManager.instance.enabled = true;
            RelayManager.instance.joinInput = SceneLoadManager.joinCode;
            await RelayManager.instance.JoinGame();

            //GameManager.instance.BeginGame();
            whileSceneLoading.SetActive(false);
        }
        else
        {
            //TEST

            if (HotseatScreenManager.instance != null)
            {
                HotseatScreenManager.instance.enabled = false;
                HotseatScreenManager.instance = null;
            }
            
            if (RelayManager.instance != null)
            {
                RelayManager.instance.enabled = false;
                RelayManager.instance = null;
            }

            GameManager.instance.BeginGame();
            whileSceneLoading.SetActive(false);
        }
    }
}
