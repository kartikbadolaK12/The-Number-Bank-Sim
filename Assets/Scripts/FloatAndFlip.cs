using UnityEngine;
using System.Collections;

public class FloatAndFlip : MonoBehaviour
{
    [Header("Movement")]
    public Transform target;
    public float moveDuration = 1.5f;

    [Header("Pause")]
    public float waitAtTarget = 0.5f;

    [Header("Spin First Stage (Whole Object)")]
    public int spinCount = 3;
    public float spinDuration = 1f;
    public Vector3 spinAxis = Vector3.up;

    [Header("Reveal GameObjects")]
    public GameObject firstObject;
    public GameObject secondObject;
    public GameObject finalObject;
    public GameObject extraHideObject;

    [Header("After Scale Show")]
    public GameObject anotherShowObject;   // ⭐ will be shown after scale is done

    [Header("Final Rotation (Whole Object)")]
    public float finalDelay = 1f;
    public float finalSpinDuration = 0.4f;
    public int finalSpinCount = 1;
    public Vector3 finalSpinAxis = Vector3.up;

    [Header("Final Scale")]
    public float finalScaleTarget = 3f;
    public float finalScaleDuration = 0.3f;

    public bool playOnStart = true;

    private Vector3 startPos;
    public AudioManager audioManager;
    public AudioClip AudioClip;
    private void Start()
    {
        startPos = transform.position;
        if (playOnStart)
            StartSequence();
    }

    public void StartSequence()
    {
        StopAllCoroutines();
        StartCoroutine(SequenceRoutine());
    }

    private IEnumerator SequenceRoutine()
    {
        // 1. Move to target
        Vector3 from = startPos;
        Vector3 to = target.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            transform.position = Vector3.Lerp(from, to, EaseInOutQuad(t));
            yield return null;
        }

        // 2. Wait
        yield return new WaitForSeconds(waitAtTarget);

        // 3. First spin
        float totalAngle1 = 360f * spinCount;
        float rotated1 = 0f;
        float speed1 = totalAngle1 / spinDuration;

        while (rotated1 < totalAngle1)
        {
            float step = speed1 * Time.deltaTime;
            transform.Rotate(spinAxis, step, Space.Self);
            rotated1 += step;
            yield return null;
        }

        // 4. first → second
        if (firstObject != null) firstObject.SetActive(false);
        if (secondObject != null) secondObject.SetActive(true);

        //Audio
        audioManager.StopAudio();
        audioManager.PlayAudio(AudioClip);
        // 5. Final delay
        yield return new WaitForSeconds(finalDelay);

        // 6. Final rotation
        float totalAngle2 = 360f * finalSpinCount;
        float rotated2 = 0f;
        float speed2 = totalAngle2 / finalSpinDuration;

        while (rotated2 < totalAngle2)
        {
            float step = speed2 * Time.deltaTime;
            transform.Rotate(finalSpinAxis, step, Space.Self);
            rotated2 += step;
            yield return null;
        }

        yield return null;

        // 7. Reveal: second → final, hide extra
        if (secondObject != null) secondObject.SetActive(false);
        if (finalObject != null) finalObject.SetActive(true);
        if (extraHideObject != null) extraHideObject.SetActive(false);

        // 8. Scale final object
        if (finalObject != null)
            yield return StartCoroutine(ScaleSmooth(finalObject.transform));

        // 9. ⭐ Show one more GameObject after scale has finished
        if (anotherShowObject != null)
            anotherShowObject.SetActive(true);
    }

    private IEnumerator ScaleSmooth(Transform targetTransform)
    {
        Vector3 startScale = targetTransform.localScale;
        Vector3 endScale = Vector3.one * finalScaleTarget;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / finalScaleDuration;
            float eased = EaseInOutQuad(t);
            targetTransform.localScale = Vector3.Lerp(startScale, endScale, eased);
            yield return null;
        }

        targetTransform.localScale = endScale;
    }

    private float EaseInOutQuad(float x)
    {
        x = Mathf.Clamp01(x);
        return x < 0.5f ?
            2f * x * x :
            1f - Mathf.Pow(-2f * x + 2f, 2) / 2f;
    }
}
