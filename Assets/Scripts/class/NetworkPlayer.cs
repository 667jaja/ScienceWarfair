using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public struct NetworkPlayer
{
    public NetworkPlayer(Player player)
    {
        playerData = player.playerData;
        playerName = player.name;
        maxMoney = player.maxMoney;
        id = player.id;
        sciencePoints = player.sciencePoints;
        money = player.Money;
        actionPoints = player.actionPoints;
        deck = player.deck;
        hand = player.hand;
        units = player.units;
    }

    public PlayerData playerData;
    public string playerName;
    public int maxMoney;
    public int id;
    public int sciencePoints;
    public int money;
    public int actionPoints;


    public List<CardData> deck;
    public List<Card> hand;

    public Card[,] units;
}
