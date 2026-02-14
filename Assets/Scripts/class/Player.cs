using UnityEngine;
using System.Collections.Generic;

public class Player
{
    //private PlayerData playerData;
    public Player(PlayerData playerData, int newId)
    {
        id = newId;

        if (playerData != null)
        {
            //this.playerData = playerData;
            name = playerData.playerName;
            rawDeck = playerData.deck;
        }

    }
    public string name;
    public int maxMoney = 10;
    public int id;
    public int sciencePoints;
    private int money;
    public int actionPoints;

    public List<CardData> rawDeck;
    public List<CardData> deck = new List<CardData>();
    public List<Card> hand = new List<Card>();
    public Card[,] units = new Card[3, 3];

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
