using UnityEngine;
using System.Collections;

public class RotateFastToSlow : MonoBehaviour
{
    [Header("Rotation speeds")]
    public float fastSpeed = 360f;      // degrees per second (start)
    public float slowSpeed = 30f;       // degrees per second (end slow speed)
    public float decelerationTime = 2.5f; // how many seconds to slow down

    [Header("Wait time before restarting")]
    public float waitTime = 2f;

    private Coroutine rotateRoutine;

    void OnEnable()
    {
        rotateRoutine = StartCoroutine(RotateLoop());
    }

    IEnumerator RotateLoop()
    {
        while (true)
        {
            // Reset timer
            float t = 0f;

            // 🔥 Phase 1 — Fast rotation → slowly reducing speed
            while (t < decelerationTime)
            {
                float speed = Mathf.Lerp(fastSpeed, slowSpeed, t / decelerationTime);
                transform.Rotate(0f, 0f, speed * Time.deltaTime);
                t += Time.deltaTime;
                yield return null;
            }

            // ⏸ Phase 2 — stop spinning slowly (if you want it to stop completely)
            float timer = 0f;
            while (timer < waitTime)
            {
                // A very small rotation or none
                transform.Rotate(0f, 0f, slowSpeed * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
