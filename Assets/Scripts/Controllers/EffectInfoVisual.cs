using UnityEngine;
using TMPro;

public class EffectInfoVisual : MonoBehaviour
{
    [SerializeField] private TMP_Text type, cooldown, cardInstanceId;
    [SerializeField] private GameObject cooldownLabel;
    [SerializeField] private TMP_Text cardHealth, cardCost, cardIq, cardDesc;
    [SerializeField] private GameObject disabledCover;
    public void InitiateEF(string typeVal)
    {
        type.text = typeVal;

    }
    public void InitiateET(string typeVal, int cooldownVal, bool noCountDown, int cardInstanceIdVal, bool isDisabled)
    {
        type.text = typeVal;
        if (noCountDown) 
        {
            cooldown.gameObject.SetActive(false);
            cooldownLabel.SetActive(false);
        }
        cooldown.text = cooldownVal.ToString();
        cardInstanceId.text = cardInstanceIdVal.ToString();
        disabledCover.SetActive(isDisabled);
    }
    public void InitiateCC(string ccName, int health, int cost, int iq, string description)
    {
        type.text = ccName;
        cardHealth.text = health.ToString();
        cardCost.text = cost.ToString();
        cardIq.text = iq.ToString();
        cardDesc.text = description;
    }
}
