using UnityEngine;
using TMPro;

public class EffectInfoVisual : MonoBehaviour
{
    [SerializeField] private TMP_Text type, EfType, cooldown;
    [SerializeField] private GameObject cooldownLabel;
    [SerializeField] private TMP_Text cardHealth, cardCost, cardIq, cardDesc;
    [SerializeField] private GameObject disabledCover;

    public void InitiateET(string typeVal, string EFtypeVal, int cooldownVal, bool noCountDown, bool isDisabled)
    {
        type.text = typeVal;
        EfType.text = EFtypeVal;

        if (noCountDown) 
        {
            cooldown.gameObject.SetActive(false);
            cooldownLabel.SetActive(false);
        }
        cooldown.text = cooldownVal.ToString();
        disabledCover.SetActive(isDisabled);
    }
    public void InitiateCC(string ccName, int health, int cost, int iq, string description)
    {
        type.text = ccName;
        cardHealth.text = health.ToString();
        cardCost.text = cost.ToString();
        cardIq.text = iq.ToString();
        cardDesc.text = description;
        disabledCover.SetActive(false);
    }
}
