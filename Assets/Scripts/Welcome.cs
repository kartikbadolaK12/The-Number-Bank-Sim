using System.Collections;
using UnityEngine;

public class Welcome : MonoBehaviour
{
    public AudioManager audioManager;
    public AudioClip audioClip;
    public float delay = 1f; // delay in seconds
    public GameObject InstructionsPanel;

    void Start()
    {
        StartCoroutine(PlayAudioWithDelay());
    }

    IEnumerator PlayAudioWithDelay()
    {
        yield return new WaitForSeconds(delay);
        audioManager.PlayAudio(audioClip);
        InstructionsPanel.SetActive(true);
    }
}
