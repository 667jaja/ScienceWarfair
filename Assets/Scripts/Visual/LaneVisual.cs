using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;

public class LaneVisual : MonoBehaviour
{
    [SerializeField] private TMP_Text iqUI;
    [SerializeField] private LaneDropArea laneDropArea;
    public Animator anim;
    private int lanePos;
    private bool isDisplayPlayer;

    public void Initiate(bool newisDisplayPlayer, int newLanePos)
    {
        laneDropArea.laneId = newLanePos;
        lanePos = newLanePos;
        isDisplayPlayer = newisDisplayPlayer;
        if (isDisplayPlayer == false)
        {
            iqUI.gameObject.SetActive(false);
            laneDropArea.enabled = false;
        }
    }
    public void UpdateVisual(int overrideNumber = -1)
    {
        if (overrideNumber >= 0) iqUI.text = overrideNumber.ToString();
        iqUI.text = UnitManager.instance.CountLaneIQ((isDisplayPlayer)? GameManager.instance.displayPlayer : GameManager.instance.GetNextPlayerId(GameManager.instance.displayPlayer), lanePos).ToString();
    }
}
