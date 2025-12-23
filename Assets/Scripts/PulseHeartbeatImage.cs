using UnityEngine;
using UnityEngine.UI;

public class PulseHeartbeatImage : MonoBehaviour
{
    public Image img;                 // Assign your UI Image
    public float pulseSpeed = 2f;     // Pulse speed
    public float minIntensity = 0.7f; // 1 = normal,  >1 brighter, <1 darker
    public float maxIntensity = 1.3f;

    private Color originalColor;

    void Start()
    {
        if (img == null)
            img = GetComponent<Image>();

        originalColor = img.color;
    }

    void Update()
    {
        // 0 → 1 → 0 pulse
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

        // Scale between min and max intensity
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        // Multiply original gold color to make it brighter/dimmer
        img.color = originalColor * intensity;
    }
}
