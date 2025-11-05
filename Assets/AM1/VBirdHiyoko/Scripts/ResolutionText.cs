using UnityEngine;
using TMPro;

public class ResolutionText : MonoBehaviour
{
    TextMeshProUGUI resolutionText;

    void Awake() {
        resolutionText = GetComponent<TextMeshProUGUI>();
    }

    void FixedUpdate()
    {
        resolutionText.text = $"{Screen.currentResolution}";
    }
}
