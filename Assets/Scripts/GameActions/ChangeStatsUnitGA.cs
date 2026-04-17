using System.Collections.Generic;
using UnityEngine;

public class ChangeStatsUnitGA : GameAction
{
    public string modDescription;
    public int playerId;
    public Vector2Int position;
    public int iqChange, heathChange, costChange;
    public List<CardTag> tags;
    public List<int> ET;
    public List<Card> savedCards;

    public ChangeStatsUnitGA(string modDescription2, int playerId2, Vector2Int position2, int iqChange2, int heathChange2, int costChange2, List<CardTag> tags2 = null, List<int> ET2 = null, List<Card> savedCards2 = null)
    {
        modDescription = modDescription2;
        playerId = playerId2;
        position = position2;

        iqChange = iqChange2;
        heathChange = heathChange2;
        costChange = costChange2;

        tags = tags2;
        ET = ET2;
        savedCards = savedCards2;
    }
    // public ChangeStatsUnitGA(string modDescription2, int playerId2, Vector2Int position2, int iqChange2, int heathChange2, int costChange2)
    // {
    //     modDescription = modDescription2;
    //     playerId = playerId2;
    //     position = position2;

    //     iqChange = iqChange2;
    //     heathChange = heathChange2;
    //     costChange = costChange2;
    // }
    // public ChangeStatsUnitGA(string modDescription2, int playerId2, Vector2Int position2, List<int> ET2, Card savedCard2 = null)
    // {
    //     modDescription = modDescription2;
    //     playerId = playerId2;
    //     position = position2;

    //     ET = ET2;
    //     savedCard = savedCard2;
    // }
}
