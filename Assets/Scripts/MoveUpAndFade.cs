using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MoveUpAndFade : MonoBehaviour
{
    [Header("Move + Fade Settings")]
    public float moveDistance = 50f;   // how much it moves up
    public float duration = 1.2f;      // time for move + fade

    private TMP_Text tmpText;
    private Graphic uiGraphic;    // for Image, Text, etc.
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Try to grab whatever makes sense on this object
        tmpText = GetComponent<TMP_Text>();
        uiGraphic = GetComponent<Graphic>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If nothing found, warn and stop
        if (tmpText == null && uiGraphic == null && spriteRenderer == null)
        {
            Debug.LogError("[MoveUpAndFade] No TMP_Text, Graphic, or SpriteRenderer found on this GameObject. Cannot fade.");
            enabled = false;
            return;
        }

        StartCoroutine(Animate());
    }

    private System.Collections.IEnumerator Animate()
    {
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0f, moveDistance, 0f);

        // Get starting color from whichever component we have
        Color startColor = Color.white;

        if (tmpText != null)
            startColor = tmpText.color;
        else if (uiGraphic != null)
            startColor = uiGraphic.color;
        else if (spriteRenderer != null)
            startColor = spriteRenderer.color;

        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // Move up
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            // Fade alpha
            Color current = Color.Lerp(startColor, endColor, t);

            if (tmpText != null)
                tmpText.color = current;
            else if (uiGraphic != null)
                uiGraphic.color = current;
            else if (spriteRenderer != null)
                spriteRenderer.color = current;

            yield return null;
        }

        // Optional: disable object after fade
        gameObject.SetActive(false);
    }
}
