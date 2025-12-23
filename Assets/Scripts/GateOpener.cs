using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GateOpener : MonoBehaviour
{
    public RectTransform leftGate;
    public RectTransform rightGate;

    [Header("Settings")]
    public float openDistance = 500f;
    public float speed = 5f;

    [Header("Choose Action")]
    public bool openGate = true;

    [Header("Delays")]
    public float startDelay = 0f;       // delay before animation starts
    public float sceneDelay = 0f;       // delay AFTER animation finishes before loading scene

    [Header("Scene Change")]
    public string nextSceneName;        // leave empty if no scene change

    private Vector2 leftInitialPos;
    private Vector2 rightInitialPos;

    private bool isAnimating = false;

    public AudioManager audioManager;
    public AudioClip gateSound;

    void Start()
    {
        leftInitialPos = leftGate.anchoredPosition;
        rightInitialPos = rightGate.anchoredPosition;

        if (!openGate)
        {
            leftGate.anchoredPosition  = leftInitialPos  + new Vector2(-openDistance, 0);
            rightGate.anchoredPosition = rightInitialPos + new Vector2( openDistance, 0);
        }

        StartCoroutine(StartAfterDelay());
    }

    IEnumerator StartAfterDelay()
    {
        if (startDelay > 0)
            yield return new WaitForSeconds(startDelay);

        isAnimating = true;

            // ⭐ Scene change if assigned
            if (!string.IsNullOrEmpty(nextSceneName))
                StartCoroutine(SceneChangeRoutine());
        if (audioManager != null && gateSound != null)
        {
            audioManager.StopAudio();
            audioManager.PlayAudio(gateSound);
        }
    }

    void Update()
    {
        if (!isAnimating) return;

        Vector2 leftTarget = openGate
            ? leftInitialPos  + new Vector2(-openDistance, 0)
            : leftInitialPos;

        Vector2 rightTarget = openGate
            ? rightInitialPos + new Vector2( openDistance, 0)
            : rightInitialPos;

        leftGate.anchoredPosition  = Vector2.Lerp(leftGate.anchoredPosition,  leftTarget, Time.deltaTime * speed);
        rightGate.anchoredPosition = Vector2.Lerp(rightGate.anchoredPosition, rightTarget, Time.deltaTime * speed);

        if (Vector2.Distance(leftGate.anchoredPosition, leftTarget) < 0.1f &&
            Vector2.Distance(rightGate.anchoredPosition, rightTarget) < 0.1f)
        {
            isAnimating = false;

        }
    }

    IEnumerator SceneChangeRoutine()
    {
        if (sceneDelay > 0)
            yield return new WaitForSeconds(sceneDelay);

        SceneManager.LoadScene(nextSceneName);
    }
}
