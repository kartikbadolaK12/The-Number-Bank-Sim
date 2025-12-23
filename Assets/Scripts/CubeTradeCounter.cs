using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CubeTradeCounter : MonoBehaviour
{
    [Header("Parent that contains the placed cubes")]
    public Transform placedParent;

    [Header("Instruction Panel")]
    public GameObject instructionsPanel;
    public TextMeshProUGUI instructionText;

    [Header("Recorder's Ledger UI")]
    public TextMeshProUGUI ledgerText;

    [Header("Door Animation")]
    public DoorRodManager doorRodManager;

    public AudioManager audioManager;
    public AudioClip countAgianClip;
    public GameObject cubeStackPlaced;
    public AudioClip tradeSuccessAudio;
    public TypewriterEffect typewriter;
    public GameObject tradeButton;
    [Header("Ledger Suffix")]
    public string singleSuffix = "One";
    public string pluralSuffix = "Ones";

    [Header("UI Messages")]
    public string successMessage = "Trade Successful! You have exactly 10 Ones.";
    public string warningMessage = "Count again! You need exactly 10 Ones to make a trade.";
    private void Start()
    {
        UpdateLedger();
    }

    // Called by Trade button
    [System.Obsolete]
    public void CheckCount()
    {
        int activeCount = CountActiveChildren();
        UpdateLedger();

        if (activeCount == 10)
        {
            instructionsPanel.SetActive(true);
            doorRodManager.PlayDoorAnimation();
            cubeStackPlaced.GetComponent<SwapMover>().enabled = true;
            audioManager.StopAudio();
            audioManager.PlayAudio(tradeSuccessAudio);
            typewriter.StartTypewriter(successMessage);

            // 🔒 STOP ALL DRAGGING NOW
            DisableAllDragging();
            //Trade btn off
            tradeButton.SetActive(false);
            CancelInvoke(nameof(HidePanel));
            Invoke(nameof(HidePanel), 4f);
        }
        else if (activeCount < 10)
        {
            instructionsPanel.SetActive(true);
            instructionText.text = warningMessage;
            audioManager.StopAudio();
            audioManager.PlayAudio(countAgianClip);

            StopAllCoroutines(); // stop previous wobble if spam-clicking
            StartCoroutine(PanelWobble()); // 🔥 play wobble animation

            CancelInvoke(nameof(HidePanel));
            Invoke(nameof(HidePanel), 4f);
        }
        else // > 10
        {
            Debug.Log("Too many cubes! You can only trade with 10.");
        }
    }

    // 🔥 Wobble animation for warning
    private IEnumerator PanelWobble()
    {
        RectTransform rect = instructionsPanel.GetComponent<RectTransform>();
        Vector3 originalScale = rect.localScale;

        float time = 0f;
        float duration = 0.35f; // wobble time
        float strength = 0.12f; // wobble size

        while (time < duration)
        {
            time += Time.deltaTime;
            float damper = 1 - (time / duration); // fades wobble over time
            float wobble = Mathf.Sin(time * 30f) * strength * damper;

            rect.localScale = originalScale + new Vector3(wobble, wobble, 0);
            yield return null;
        }

        rect.localScale = originalScale; // reset perfectly
    }

    public void UpdateLedger()
    {
        int activeCount = CountActiveChildren();
        if (ledgerText != null)
        {
            string suffix = (activeCount == 1) ? singleSuffix : pluralSuffix;
            ledgerText.text = $" {activeCount} {suffix}";
        }
    }


    private int CountActiveChildren()
    {
        int activeCount = 0;
        for (int i = 0; i < placedParent.childCount; i++)
        {
            if (placedParent.GetChild(i).gameObject.activeSelf)
                activeCount++;
        }
        return activeCount;
    }

    private void HidePanel()
    {
        instructionsPanel.SetActive(false);
    }

    // 🔒 Helper: stop drag feature everywhere
    [System.Obsolete]
    private void DisableAllDragging()
    {
        DragOnlyXY[] dragScripts = FindObjectsOfType<DragOnlyXY>();
        foreach (var drag in dragScripts)
        {
            drag.enabled = false;
        }
    }
}
