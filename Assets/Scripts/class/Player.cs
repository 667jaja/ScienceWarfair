using UnityEngine;
using System.Collections.Generic;

public class Player
{
    public int id;
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
}
