using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionLogManager : MonoBehaviour
{
    public static ActionLogManager instance; 
    [SerializeField] private float logEntryDistance;
    [SerializeField] private int loggedActionObjectMax;
    [SerializeField] private int loggedActionMax;
    [SerializeField] private Transform actionLogBase;
    [SerializeField] private GameObject loggedActionPrefab;
    [SerializeField] private GameObject specialLoggedActionPrefab;
    private List<GameObject> loggedActionObjects = new List<GameObject>();
    [SerializeField] private List<String> loggedActions = new List<String>();

    void Awake()
    {
        instance = this;
    }

    public void LogAction(string action)
    {
        bool isSpecialAction = action.Contains("Game start.")||action.Contains("turn.");
        
        if (loggedActions.Count > 0 && loggedActions[loggedActions.Count-1].Equals(action))
        {
            Debug.Log("repeat action detected");
            int i = 0;
            for (i = 0; i < loggedActions.Count; i++)
            {
                if (loggedActions[loggedActions.Count-(i+1)] != action)
                {
                    i++;
                    break;
                }
            }
            if (loggedActionObjects[loggedActionObjects.Count-1].GetComponent<TextMeshProUGUI>() != null) loggedActionObjects[loggedActionObjects.Count-1].GetComponent<TextMeshProUGUI>().text = action +" " + i + " times";
        }
        else
        {
            GameObject newLoggedAction = Instantiate((isSpecialAction)?specialLoggedActionPrefab:loggedActionPrefab, actionLogBase);

            foreach (GameObject item in loggedActionObjects)
            {
                item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y + logEntryDistance, item.transform.position.z);
            }

            if (newLoggedAction.GetComponent<TextMeshProUGUI>() != null) newLoggedAction.GetComponent<TextMeshProUGUI>().text = action;

            loggedActionObjects.Add(newLoggedAction);
            if (loggedActionObjects.Count > loggedActionObjectMax)
            {
                Destroy(loggedActionObjects[0]);
                loggedActionObjects.RemoveAt(0);
            }
        }

        loggedActions.Add(action);
        if (loggedActions.Count > loggedActionMax)
        {
            loggedActions.RemoveAt(0);
        }
    }
}
