using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HotseatScreenController : MonoBehaviour
{
    public static HotseatScreenController instance;

    [SerializeField] private float appearenceDelay = 0.5f;
    [SerializeField] private string mainMenuName = "MainMenu";
    [SerializeField] private Slider scienceSlider1;
    [SerializeField] private Slider scienceSlider2;

    [SerializeField] private List<TextMeshProUGUI> player1Name;
    [SerializeField] private List<TextMeshProUGUI> player2Name;

    [SerializeField] private List<GameObject> startScreenObjects;
    [SerializeField] private List<GameObject> turnScreenObjects;
    [SerializeField] private List<GameObject> winScreenObjects;
    [SerializeField] private TextMeshProUGUI Text;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        StartBlock();

        foreach (TextMeshProUGUI textMeshPro in player1Name)
        {
            textMeshPro.text = GameManager.instance.players[0].name;
        }
        foreach (TextMeshProUGUI textMeshPro in player2Name)
        {
            textMeshPro.text = GameManager.instance.players[1].name;
        }
    }

    public IEnumerator OnTurnEnd()
    {
        if (GameManager.instance.players[0].sciencePoints >= GameManager.instance.maxSciencePoints && GameManager.instance.players[0].sciencePoints > GameManager.instance.players[1].sciencePoints)
        {
            YouWin(0);
        }
        else if (GameManager.instance.players[1].sciencePoints >= GameManager.instance.maxSciencePoints && GameManager.instance.players[1].sciencePoints > GameManager.instance.players[0].sciencePoints)
        {
            YouWin(1);
        }
        else
        {
            yield return BlockScreen();
        }
    }

    public void StartBlock()
    {
        Text.text = "Waiting for " + ((GameManager.instance.playerDatas[0].playerName != null && GameManager.instance.playerDatas[0].playerName.Length >= 1)? GameManager.instance.playerDatas[GameManager.instance.currentPlayer].playerName : "Player1") + " to Click Start";
        foreach (GameObject item in startScreenObjects)
        {
            item.SetActive(true);
        }
    }

    public IEnumerator BlockScreen()
    {
        yield return new WaitForSeconds(appearenceDelay);

        Text.text = "Waiting for " + GameManager.instance.players[GameManager.instance.currentPlayer].name + "...";
        foreach (GameObject item in turnScreenObjects)
        {
            item.SetActive(true);
        }
    }

    public void YouWin(int winnerId)
    {
        scienceSlider1.value = GameManager.instance.players[0].sciencePoints;
        scienceSlider2.value = GameManager.instance.players[1].sciencePoints;

        Text.text = "The Winner is "+ GameManager.instance.players[winnerId].name+"!";
        foreach (GameObject item in winScreenObjects)
        {
            item.SetActive(true);
        }
    }

// Buttons
    public void StartScreenButton()
    {
        GameManager.instance.BeginGame();

        foreach (GameObject item in startScreenObjects)
        {
            item.SetActive(false);
        }
    }

    public void NextGuy()
    {
        // GameManager.instance.turnCount += 1;
        // GameManager.instance.SwitchPlayer();
        foreach (GameObject item in turnScreenObjects)
        {
            item.SetActive(false);
        }
        GameManager.instance.StartTurn();
    }

    public void closeGame()
    {
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(mainMenuName);
    }
}
