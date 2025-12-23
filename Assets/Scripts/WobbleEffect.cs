using UnityEngine;
using System.Collections;

public class WobbleEffect : MonoBehaviour
{
    [Header("Wobble Settings")]
    public float delayBeforeStart = 0f;   // ⏳ public delay before wobble
    public float duration = 0.35f;        // wobble time
    public float strength = 0.12f;        // wobble intensity
    public bool resetAfter = true;        // return to original scale after wobble

    private Coroutine wobbleRoutine;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        // Automatically wobble on Start after delay
        PlayWobble();
    }

    /// <summary>
    /// Call this to play wobble once manually
    /// </summary>
    public void PlayWobble()
    {
        if (wobbleRoutine != null)
            StopCoroutine(wobbleRoutine);

        wobbleRoutine = StartCoroutine(WobbleSequence());
    }

    private IEnumerator WobbleSequence()
    {
        // ⏳ Wait before wobble starts
        if (delayBeforeStart > 0f)
            yield return new WaitForSeconds(delayBeforeStart);

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float damper = 1f - (time / duration);
            float wobble = Mathf.Sin(time * 30f) * strength * damper;

            transform.localScale = originalScale + new Vector3(wobble, wobble, 0);
            yield return null;
        }

        if (resetAfter)
            transform.localScale = originalScale;

        wobbleRoutine = null;
    }
}
