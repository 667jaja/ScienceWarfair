using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [field: SerializeField] public string playerName { get; private set; }
    [field: SerializeField] public List<CardData> deck { get; set; }

    //[field: SerializeField] public thing startingPlan { get; private set; }
    //[field: SerializeField] public otherthing characterimage { get; private set; }
    public void changeName(string newName)
    {
        playerName = newName;
    }
}
