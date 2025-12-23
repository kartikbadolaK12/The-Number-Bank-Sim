using UnityEngine;

public class StartScript : MonoBehaviour
{
    public GameObject instructionsPanel;
    public TypewriterEffect typewriter;
    public string fullText = "Drag 10 Ones to the Banker to exchange.";
    public AudioManager audioManager;
    public AudioClip audioClip;

    [Header("Delay before playback")]
    public float startDelay = 2f;

    private void Start()
    {
        Invoke(nameof(StartDialogue), startDelay);
    }

    private void StartDialogue()
    {
        // Panel
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);

        // Typewriter text
        if (typewriter != null)
            typewriter.StartTypewriter(fullText);

        // Audio
        if (audioManager != null)
        {
            audioManager.StopAudio();
            if (audioClip != null)
                audioManager.PlayAudio(audioClip);   // Play only if clip exists
        }
    }
}
