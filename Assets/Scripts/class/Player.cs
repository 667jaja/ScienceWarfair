using UnityEngine;
using System.Collections.Generic;

public class Player
{
    public int maxMoney = 10;
    public int id;
    public int sciencePoints;
    private int money;


    public List<Card> deck = new List<Card>();
    public List<Card> hand = new List<Card>();
    // public Card[] lane1 = new Card[3];
    // public List<Card> lane2 = new List<Card>();
    // public List<Card> lane3 = new List<Card>();
    public Card[,] units = new Card[3, 3];
    public Player(int numberOfPlayers)
    {
        id = numberOfPlayers;
    }

    public int Money
    {
        get
        {
            return money;
        }
        set
        {
            if (value > maxMoney)
            {
                money = maxMoney;
            }
            else
            {
                money = value;
            }
        }
    }
}
