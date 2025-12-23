using UnityEngine;
using System.Collections;

public class Starter : MonoBehaviour
{
    [Header("Delay times (seconds)")]
    public float delay1 = 1f;
    public float delay2 = 1f;

    [Header("Messages")]
    public string message1 = "Message 1";
    public string message2 = "Message 2";

    [Header("Audio")]
    public AudioManager audioManager;
    public AudioClip audioClip1;
    public AudioClip audioClip2;

    [Header("UI")]
    public TypewriterEffect typewriter;
    public GameObject instructionsPanel;

    void Start()
    {
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        // ───── FIRST PART ─────
        yield return new WaitForSeconds(delay1);

        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);

        Debug.Log(message1);

        if (audioManager != null && audioClip1 != null)
            audioManager.PlayAudio(audioClip1);

        if (typewriter != null)
            typewriter.StartTypewriter(message1);

        // ───── SECOND PART ─────
        yield return new WaitForSeconds(delay2);

        Debug.Log(message2);

        if (audioManager != null && audioClip2 != null)
            audioManager.PlayAudio(audioClip2);

        if (typewriter != null)
            typewriter.StartTypewriter(message2);
    }
}
