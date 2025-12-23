using UnityEngine;
using System.Collections;

public class ActivateChildrenOneByOne : MonoBehaviour
{
    public float delay = 0.5f;    // Speed of showing children (time between each)
    public bool playOnStart = true;
public AudioManager audioManager;
public AudioClip chainSound;
    private void Start()
    {
        if (playOnStart)
            StartSequence();

    }

    public void StartSequence()
    {
        audioManager.StopAudio();
        audioManager.PlayAudio(chainSound);
        StopAllCoroutines();
        StartCoroutine(ActivateRoutine());
    }

    private IEnumerator ActivateRoutine()
    {
        // Disable all children first
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        // Activate children starting from the LAST → FIRST
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(delay);
        }
    }
}
