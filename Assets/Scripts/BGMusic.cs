using UnityEngine;

public class BGMusic : MonoBehaviour
{
    private static BGMusic instance;

    public AudioSource audioSource; // put music here

    void Awake()
    {
        // If another instance already exists → destroy duplicate
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // keep this object across scenes
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}
