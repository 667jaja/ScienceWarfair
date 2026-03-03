using UnityEngine;
using TMPro;

public class EffectInfoVisual : MonoBehaviour
{
    [SerializeField] private TMP_Text type, cooldown, cardInstanceId;
    [SerializeField] private GameObject disabledCover;
    [SerializeField] private bool isET;
    public void Initiate(string typeVal, int cooldownVal, int cardInstanceIdVal, bool isDisabled)
    {
        type.text = typeVal;
        if (isET)
        {
            cooldown.text = cooldownVal.ToString();
            cardInstanceId.text = cardInstanceIdVal.ToString();
            disabledCover.SetActive(isDisabled);
        }
    }
}
