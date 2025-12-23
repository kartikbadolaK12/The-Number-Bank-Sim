using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceSlot : MonoBehaviour, IDropHandler
{
    [Header("Correct place type for this slot")]
    public PlaceType requiredType;

    [Header("If false, this slot never accepts tiles (fixed label slot).")]
    public bool acceptsDrag = true;

    [Header("Messages")]
    [TextArea] public string correctMessage = "Correct!";
    [TextArea] public string tooHighMessage = "Way too high. Start smaller.";
    [TextArea] public string tooLowMessage = "Way too small. Start larger.";
    [TextArea] public string arabMessage = "Too high! That comes after the chart ends.";

    [Header("Audio")]
    public AudioManager audioManager;
    public AudioClip correctSound;
    public AudioClip tooHighSound;
    public AudioClip tooLowSound;
    public AudioClip arabSound;

    [Header("UI")]
    public GameObject instructionPanel;
    public TextMeshProUGUI instructionText;

    [Header("Final Manager")]
    public GameObject finalManager;   // Will be activated when all slots are correct

    [Header("Final Manager Delay")]
    public float completionDelay = 1.5f; // wait time before finalManager shows

    [Header("Coin Popup")]
    public GameObject coinObject;     // Shown when this slot gets the correct tile

    [HideInInspector] public PlaceNameTile currentTile;

    // ---------- Global tracking ----------
    // Counts how many slots in total (that accept drag)
    public static int totalSlots = 0;

    // Counts how many of those are currently correctly filled
    public static int filledCorrectSlots = 0;

    // Per-slot flag: is THIS slot currently correct?
    private bool isFilledCorrect = false;

    private void Start()
    {
        // Only count slots that actually accept tiles
        if (acceptsDrag)
        {
            totalSlots++;
        }

        // Make sure coin is hidden at start (optional)
        if (coinObject != null)
        {
            coinObject.SetActive(false);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!acceptsDrag) return;
        if (eventData.pointerDrag == null) return;

        var tile = eventData.pointerDrag.GetComponent<PlaceNameTile>();
        if (tile == null) return;

        // ✅ CORRECT TILE
        if (tile.placeType == requiredType)
        {
            // If something already here and it's a different tile, send that back
            if (currentTile != null && currentTile != tile)
            {
                currentTile.ResetPosition();
            }

            // Place this tile here
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = Vector3.zero;
            currentTile = tile;

            // 🔔 Show coin when correctly placed
            if (coinObject != null)
            {
                coinObject.SetActive(true);
            }

            // If this slot was not correct before, now it becomes correct
            if (!isFilledCorrect)
            {
                isFilledCorrect = true;
                filledCorrectSlots++;
                CheckCompletion();
            }

            // Show correct message
            if (instructionPanel != null) instructionPanel.SetActive(true);
            if (instructionText != null) instructionText.text = correctMessage;
            Debug.Log($"[Slot {name}] {correctMessage}");

            if (audioManager != null)
            {
                audioManager.StopAudio();
                audioManager.PlayAudio(correctSound);
            }

            return;
        }

        // ❌ WRONG TILE
        // If this slot was previously correct and the player is dragging
        // a WRONG tile over it, we do NOT change the slot's state,
        // because we never assign wrong tile to this slot (we reset it later).

        if (tile.placeType == PlaceType.Arab)
        {
            if (instructionPanel != null) instructionPanel.SetActive(true);
            if (instructionText != null) instructionText.text = arabMessage;
            Debug.Log($"[Slot {name}] {arabMessage}");

            if (audioManager != null)
            {
                audioManager.StopAudio();
                audioManager.PlayAudio(arabSound);
            }
        }
        else
        {
            int tileRank = (int)tile.placeType;
            int requiredRank = (int)requiredType;

            if (tileRank > requiredRank)
            {
                if (instructionPanel != null) instructionPanel.SetActive(true);
                if (instructionText != null) instructionText.text = tooHighMessage;
                Debug.Log($"[Slot {name}] {tooHighMessage}");

                if (audioManager != null)
                {
                    audioManager.StopAudio();
                    audioManager.PlayAudio(tooHighSound);
                }
            }
            else
            {
                if (instructionPanel != null) instructionPanel.SetActive(true);
                if (instructionText != null) instructionText.text = tooLowMessage;
                Debug.Log($"[Slot {name}] {tooLowMessage}");

                if (audioManager != null)
                {
                    audioManager.StopAudio();
                    audioManager.PlayAudio(tooLowSound);
                }
            }
        }

        // Always send wrong tile back
        tile.ResetPosition();
    }

    private void CheckCompletion()
    {
        // Only trigger when all slots that accept drag are correct
        if (filledCorrectSlots >= totalSlots && totalSlots > 0)
        {
            Debug.Log("🎉 All slots completed!");
            StartCoroutine(ShowFinalAfterDelay());
        }
    }

    private IEnumerator ShowFinalAfterDelay()
    {
        yield return new WaitForSeconds(completionDelay);

        if (instructionPanel != null)
            instructionPanel.SetActive(false);

        if (finalManager != null)
        {
            finalManager.SetActive(true);
        }
    }

    // OPTIONAL: if you ever reset level, you might want a static reset helper
    public static void ResetGlobalCounters()
    {
        totalSlots = 0;
        filledCorrectSlots = 0;
    }
}
