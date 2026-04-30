using TMPro;
using UnityEngine;

public class GDPRUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gdprText;

    public void SetGDPRText(bool gdpr)
    {
        _gdprText.text = "Non-GDPR compliant: " + (gdpr ? "TRUE" : "FALSE");
    }
}
