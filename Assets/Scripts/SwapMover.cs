using UnityEngine;

public class SwapMover : MonoBehaviour
{
    [Header("Move target")]
    public Transform targetPoint;

    [Header("Objects to toggle")]
    public GameObject objectToHide;   // moving object
    public GameObject objectToShow;   // object shown after move

    [Header("Speed")]
    public float moveSpeed = 3f;

    [Header("Delay before starting move (seconds)")]
    public float delayBeforeStart = 1f;

    private Transform mover;
    private float delayTimer;
    private bool isMoving = false;
    private bool hasSwapped = false;

    private void Awake()
    {
        mover = (objectToHide != null) ? objectToHide.transform : transform;
    }

    private void OnEnable()
    {
        // When script is enabled (by button or at start) -> reset state
        delayTimer = delayBeforeStart;
        isMoving = false;
        hasSwapped = false;
    }

    private void Update()
    {
        if (targetPoint == null || mover == null || hasSwapped)
            return;

        // 1) Handle initial delay
        if (!isMoving)
        {
            if (delayTimer > 0f)
            {
                delayTimer -= Time.deltaTime;
                return; // still waiting
            }

            // Delay finished -> start moving
            isMoving = true;
            return; // start actual movement next frame (clean)
        }

        // 2) Move towards target each frame
        mover.position = Vector3.MoveTowards(
            mover.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );

        // 3) Check if reached target
        if (Vector3.Distance(mover.position, targetPoint.position) <= 0.001f)
        {
            // Snap to exact target to be safe
            mover.position = targetPoint.position;

            // Swap visibility
            if (objectToHide != null)
                objectToHide.SetActive(false);

            if (objectToShow != null)
                objectToShow.SetActive(true);

            hasSwapped = true;

            // Optional: stop running this script after done
            // enabled = false;

            Debug.Log("[SwapMover] Move + swap complete.");
        }
    }
}
