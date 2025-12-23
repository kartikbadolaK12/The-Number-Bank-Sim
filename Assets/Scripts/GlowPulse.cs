using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GlowPulse : MonoBehaviour
{
    [Header("Glow Settings")]
    public Color glowColor = Color.yellow;  // Color of the glow
    public float minIntensity = 0.2f;      // Lowest glow
    public float maxIntensity = 2f;        // Highest glow
    public float pulseSpeed = 2f;          // Speed of pulsing

    private Renderer rend;
    private Material mat;
    private Color baseEmissionColor;

    private void Awake()
    {
        rend = GetComponent<Renderer>();

        // Use the *instance* of the material so we don't edit shared material
        mat = rend.material;

        // Make sure emission is on
        mat.EnableKeyword("_EMISSION");

        // Save original emission
        baseEmissionColor = mat.GetColor("_EmissionColor");
    }

    private void Update()
    {
        // Ping-pong between 0 and 1
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        Color finalColor = glowColor * intensity;

        mat.SetColor("_EmissionColor", finalColor);
    }

    private void OnDisable()
    {
        // Reset emission when disabled (optional)
        if (mat != null)
            mat.SetColor("_EmissionColor", baseEmissionColor);
    }
}
