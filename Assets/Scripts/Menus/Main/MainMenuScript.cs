using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public static bool hasResetName = false;
    [SerializeField] private string levelSelectName;
    [SerializeField] private string deckMakerName;
    [SerializeField] private string gameBoardName;
    [SerializeField] private List<Card> defaultDeck;
    [SerializeField] private List<PlayerData> playerDatas;
    [SerializeField] private TMP_InputField joinInput;
    [SerializeField] private TMP_Dropdown SelectedPlayerInput;

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
        SelectedPlayerInput.options = new();
        foreach (PlayerData data in playerDatas)
        {
            if (data.playerName != null && data.playerName.Length > 0)
            {
                SelectedPlayerInput.options.Add(new TMP_Dropdown.OptionData(data.playerName));
            }
        }
    }
    public void PlayerNamesUpdated()
    {
        SelectedPlayerInput.options = new();
        foreach (PlayerData data in playerDatas)
        {
            if (data.playerName != null && data.playerName.Length > 0)
            {
                SelectedPlayerInput.options.Add(new TMP_Dropdown.OptionData(data.playerName));
            }
        }
        SelectedPlayerInput.value = 0;
    }
    public void LoadLocal()
    {
        SceneLoadManager.instance.LoadGameBoard(BattleType.HotSeat);
    }
    public void LoadOnline()
    {
        if (joinInput.text.Length == 6)
        {
            SceneLoadManager.instance.LoadGameBoard(BattleType.OnlineJoin, SelectedPlayerInput.value, joinInput.text);
        }
        else
        {
            SceneLoadManager.instance.LoadGameBoard(BattleType.OnlineHost, SelectedPlayerInput.value);
        }
    }

    public void CloseTab()
    {
        this.gameObject.SetActive(false);
    }
}
