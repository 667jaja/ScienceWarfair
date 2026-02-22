using UnityEngine;
using System.Collections.Generic;

public class GameLaunchInit : MonoBehaviour
{
    public static bool hasResetName = false;
    [SerializeField] private List<PlayerData> playerDatas;

    void Start()
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
       SceneLoadManager.instance.LoadMainMenu(); 
    }
}
