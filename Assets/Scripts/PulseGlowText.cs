using UnityEngine;
using TMPro;

public class PulseGlowText : MonoBehaviour
{
    public TextMeshProUGUI tmpText;       // Assign your text
    public Color glowColor = Color.cyan;  // Glow color
    public float pulseSpeed = 2f;         // Speed of glow
    public float minGlow = 0.3f;          // Minimum intensity
    public float maxGlow = 1.2f;          // Maximum intensity

    private float glow;
    private Color originalColor;

    void Start()
    {
        if (tmpText == null)
            tmpText = GetComponent<TextMeshProUGUI>();

        originalColor = tmpText.color;
    }

    void Update()
    {
        // Ping-pong value between 0 and 1
        glow = Mathf.Lerp(minGlow, maxGlow, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);

        // Blend original color with glow color
        tmpText.color = Color.Lerp(originalColor, glowColor, glow);
    }
}
