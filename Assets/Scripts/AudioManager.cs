using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // public static AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource audioSource;  // Assign any AudioSource in Inspector

    void Awake()
    {
        // // Make it a Singleton (only one instance stays)
        // if (Instance == null)
        // {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject); // keep it across scenes
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Plays a single audio clip once.
    /// </summary>
    public void PlayAudio(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Tried to play a null clip!");
            return;
        }

        if (audioSource == null)
        {
            Debug.LogWarning("AudioManager: No AudioSource assigned!");
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Plays an audio clip with volume control.
    /// </summary>
    public void PlayAudio(AudioClip clip, float volume)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    /// <summary>
    /// Stops any currently playing audio.
    /// </summary>
    public void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }
}
