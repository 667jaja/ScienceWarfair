using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public static bool hasResetName = false;
    [SerializeField] private string levelSelectName;
    [SerializeField] private string deckMakerName;
    [SerializeField] private string gameBoardName;
    [SerializeField] private List<Card> defaultDeck;
    [SerializeField] private List<PlayerData> playerDatas;
    // private DeckInfo deckInfo;

    void Awake()
    {
        if (!hasResetName)
        {
            foreach (PlayerData data in playerDatas)
            {
                data.changeName("");
                data.deck = new();
            }
            hasResetName = true;
        }
    }
    public void LoadLevelSelect()
    {
        SceneManager.LoadSceneAsync(levelSelectName);
    }
    
    public void LoadDeckMaker()
    {
        SceneManager.LoadSceneAsync(deckMakerName);
    }

    public void LoadGameBoard()
    {
        // if (DeckInfo.deck1.Count < 20)
        // {
        //     DeckInfo.deck1 = defaultDeck;
        // }
        // if (DeckInfo.deck2.Count < 20)
        // {
        //     DeckInfo.deck2 = defaultDeck;
        // }
        SceneManager.LoadSceneAsync(gameBoardName);
    }

    public void CloseTab()
    {
        this.gameObject.SetActive(false);
    }
    
    public void QuitApplication()
    {
        Application.Quit();
    }
}
