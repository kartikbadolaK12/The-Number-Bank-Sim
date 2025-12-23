using UnityEngine;

public class FloatingNumbersBG : MonoBehaviour
{
    [Header("Rotation")]
    public float maxRotateAngle = 5f;      // how much it tilts left/right
    public float rotateSpeed = 0.5f;       // how fast it tilts

    [Header("Up & Down Float (optional)")]
    public float floatAmplitude = 10f;     // how many pixels up/down
    public float floatSpeed = 0.5f;        // how fast it floats

    RectTransform rect;
    Vector2 startAnchoredPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            startAnchoredPos = rect.anchoredPosition;
        }
    }

    void Update()
    {
        float t = Time.time;

        // 🔁 Smooth left–right rotation with sine wave
        float angle = Mathf.Sin(t * rotateSpeed) * maxRotateAngle;

        if (rect != null)
        {
            rect.localRotation = Quaternion.Euler(0f, 0f, angle);

            // ⬆⬇ optional floating movement
            float offsetY = Mathf.Sin(t * floatSpeed) * floatAmplitude;
            rect.anchoredPosition = startAnchoredPos + new Vector2(0f, offsetY);
        }
        else
        {
            // fallback if it's a normal Transform (world sprite)
            transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            float offsetY = Mathf.Sin(t * floatSpeed) * floatAmplitude;
            transform.position += new Vector3(0f, offsetY, 0f);
        }
    }
}
