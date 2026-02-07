using UnityEngine;
using UnityEngine.SceneManagement;

public enum BattleType
{
    Test = 0,
    HotSeat = 1,
    AgainstBot = 2,
    OnlineHost = 3,
    OnlineJoin = 4
}

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager instance;
    public static BattleType gameType;
    public static string joinCode;
    public static int selectedPlayerData;
    //public static int level;
    [SerializeField] private string gameBoardName = "GameBoard";
    [SerializeField] private string deckMakerName = "DeckBuilder";
    [SerializeField] private string levelSelectName = "LevelSelect";
    [SerializeField] private string loadScreenName = "LoadScreen";
    [SerializeField] private string mainMenuName = "MainMenu";

    void Awake()
    {
        instance = this;
    }
    public void LoadMainMenu()
    {
        // SceneManager.LoadSceneAsync(loadScreenName);
        SceneManager.LoadSceneAsync(mainMenuName);
    }
    public void LoadLevelSelect()
    {
        // SceneManager.LoadSceneAsync(loadScreenName);
        SceneManager.LoadSceneAsync(levelSelectName);
    }
    
    public void LoadDeckMaker()
    {
        // SceneManager.LoadSceneAsync(loadScreenName);
        SceneManager.LoadSceneAsync(deckMakerName);
    }

    public void LoadGameBoard(BattleType gameType1, int selectedPlayerData1 = 0, string joinCode1 = null)//, int level = -1
    {
        gameType = gameType1;
        joinCode = joinCode1;
        selectedPlayerData = selectedPlayerData1;
        // SceneManager.LoadSceneAsync(loadScreenName);
        SceneManager.LoadSceneAsync(gameBoardName);
    }
    public void QuitApplication()
    {
        Application.Quit();
    }
}
