using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CubePlaceSwitcher : MonoBehaviour
{
    [Header("Placement Zone (Collider)")]
    public Collider placeZone;              // Drag your placement zone collider here

    [Header("Placed Stack Parent (CubesStackPlaced)")]
    public Transform placedParent;          // Drag CubesStackPlaced here

    [Header("UI Warning Panel")]
    public GameObject instructionsPanel;
    public TextMeshProUGUI instructionText;
    public AudioManager audioManager;
    public AudioClip audioClip;
    public GameObject tray;

    // Shared index for all cubes (sequence)
    private static int s_revealIndex = 0;

    private bool alreadyPlaced = false;
    private Collider myCollider;

    // 🔹 store original transform to reset when dropped outside zone
    private Vector3 originalPosition;
    private Quaternion originalRotation;

  void Awake()
{
    myCollider = GetComponent<Collider>();

    Rigidbody rb = GetComponent<Rigidbody>();
    rb.useGravity = true;   // ❌ no gravity
    rb.isKinematic = false;   // ❌ not simulated by physics

    originalPosition = transform.position;
    originalRotation = transform.rotation;
}

    void Start()
    {
        ResetSequence();
    }
    // 🔥 Snap on mouse/touch release
    [System.Obsolete]
    private void OnMouseUp()
    {
        if (alreadyPlaced) return;
        if (placeZone == null || placedParent == null) return;

        // Check if cube overlaps the placeZone on release
        if (!IsOverPlaceZone())
        {
            // ❗ Not on place zone → return to original position
            ResetToOriginal();
            return;
        }

        // ---- CHECK IF VAULT IS FULL BEFORE REVEALING ----
        if (s_revealIndex >= 10) // vault capacity = 10 cubes
        {
            if (instructionsPanel != null) instructionsPanel.SetActive(true);
            if (instructionText != null)
                instructionText.text = "Stop! You have 10. You must trade now!";

            Debug.Log("Stop! You have 10. You must trade now!");

            if (audioManager != null && audioClip != null)
            {
                audioManager.StopAudio();
                audioManager.PlayAudio(audioClip);
            }

            // 🔥 Play wobble warning animation
            StopAllCoroutines();
            StartCoroutine(PanelWobble());

            // 🔥 Auto hide panel after 4 seconds
            CancelInvoke(nameof(HidePanel));
            // Invoke(nameof(HidePanel), 4f);

            // Hide whole cube
            gameObject.SetActive(false);

            DisableAllDrag();
            return;
        }

        // ---- NORMAL REVEAL ----
        if (s_revealIndex < placedParent.childCount)
        {
            Transform cubeToReveal = placedParent.GetChild(s_revealIndex);
            cubeToReveal.gameObject.SetActive(true);
        }

        s_revealIndex++;

        // ✅ When exactly 10 cubes have been revealed/placed
        if (s_revealIndex == 10)
        {
            Debug.Log("its 10 good");
            if (tray != null)
            {
                var glow = tray.GetComponent<GlowPulse>();
                if (glow != null) glow.enabled = true;
            }
        }

        // 🔥 Update Recorder's Ledger after placement
        CubeTradeCounter counter = FindObjectOfType<CubeTradeCounter>();
        if (counter != null)
            counter.UpdateLedger();

        // Hide this dragged cube (it has been "used")
        gameObject.SetActive(false);
        alreadyPlaced = true;
    }

    // Bounds overlap check
    private bool IsOverPlaceZone()
    {
        if (myCollider == null || placeZone == null) return false;
        return myCollider.bounds.Intersects(placeZone.bounds);
    }

    // 🔙 Reset cube back to where it started
    private void ResetToOriginal()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

    [System.Obsolete]
    private void DisableAllDrag()
    {
        CubePlaceSwitcher[] allSwitchers = FindObjectsOfType<CubePlaceSwitcher>();
        foreach (var sw in allSwitchers)
            sw.alreadyPlaced = true;

        DragOnlyXY[] dragScripts = FindObjectsOfType<DragOnlyXY>();
        foreach (var drag in dragScripts)
            drag.enabled = false;
    }

    public static void ResetSequence()
    {
        s_revealIndex = 0;
    }

    private void HidePanel()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }

    // 🔥 Same wobble as in CubeTradeCounter
    private IEnumerator PanelWobble()
    {
        if (instructionsPanel == null) yield break;

        RectTransform rect = instructionsPanel.GetComponent<RectTransform>();
        if (rect == null) yield break;

        Vector3 originalScale = rect.localScale;

        float time = 0f;
        float duration = 0.35f;  // wobble time
        float strength = 0.12f;  // wobble size

        while (time < duration)
        {
            time += Time.deltaTime;
            float damper = 1f - (time / duration);          // fade out
            float wobble = Mathf.Sin(time * 30f) * strength * damper;

            rect.localScale = originalScale + new Vector3(wobble, wobble, 0f);
            yield return null;
        }

        rect.localScale = originalScale; // reset
    }
}
