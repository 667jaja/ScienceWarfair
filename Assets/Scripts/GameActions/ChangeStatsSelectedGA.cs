using System.Collections.Generic;
using UnityEngine;

public class ChangeStatsSelectedGA : GameAction
{
    public string modDescription;
    public int iqChange, heathChange, costChange;
    public List<CardTag> tags;
    public int ET;
    public Card savedCard;
    public bool isUnited;
    public ChangeStatsSelectedGA(string modDescription2, int iqChange2, int heathChange2, int costChange2, List<CardTag> tags2, int ET2 = -1, Card savedCard2 = null, bool isUnited2 = false)
    {
        modDescription = modDescription2;

        iqChange = iqChange2;
        heathChange = heathChange2;
        costChange = costChange2;
        
        tags = tags2;
        ET = ET2;
        savedCard =savedCard2;
        isUnited = isUnited2;
    }
    // public ChangeStatsSelectedGA(string modDescription2, int iqChange2, int heathChange2, int costChange2)
    // {
    //     modDescription = modDescription2;

    //     iqChange = iqChange2;
    //     heathChange = heathChange2;
    //     costChange = costChange2;
    // }
    // public ChangeStatsSelectedGA(string modDescription2, int ET2, Card savedCard2 = null)
    // {
    //     modDescription = modDescription2;

    //     ET = ET2;
    //     savedCard = savedCard2;
    // }
}
