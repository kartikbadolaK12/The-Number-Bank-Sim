using UnityEngine;

public class PlayAnimationInterval : MonoBehaviour
{
    public Animator animator;
    public AnimationClip clip;            // animation to play
    public float intervalSeconds = 5f;    // play every X seconds
    public string idleStateName = "New State"; // idle/default state

    private float timer;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // Play once immediately when game starts
        PlayOnce();

        // Start countdown for next time
        timer = intervalSeconds;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PlayOnce();
            timer = intervalSeconds;
        }
    }

    void PlayOnce()
    {
        animator.CrossFade(clip.name, 0f, 0, 0f);
        Invoke(nameof(ReturnToIdle), clip.length);
    }

    void ReturnToIdle()
    {
        animator.CrossFade(idleStateName, 0f, 0, 0f);
    }
}
