using UnityEngine;
using System.Collections.Generic;

public class Player
{
    public int id;
    public List<Card> deck = new List<Card>();
    public List<Card> hand = new List<Card>();
    public List<Card> played = new List<Card>();
    public Player(int numberOfPlayers)
    {
        id = numberOfPlayers;
    }
}
