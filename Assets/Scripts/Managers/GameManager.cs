using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //manages overarching elements of a game such as players, loss, victory, turn start, turn end
    public static GameManager instance;
    [SerializeField] private int playerCount = 2;    
    public List<Player> players;
    public int currentPlayer;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        BeginGame();
    }
    private void BeginGame()
    {
        players = new();
        for (int i = 0; i < playerCount; i++ )
        {
            players.Add(new Player(i));
            CardManager.instance.CreateDeck();
        }
    }
}
