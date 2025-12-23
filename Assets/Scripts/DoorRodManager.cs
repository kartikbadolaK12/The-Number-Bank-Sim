using UnityEngine;

public class DoorRodManager : MonoBehaviour
{
    private Animator anim;

    [Header("Rod movement")]
    public GameObject rod;            // moving rod
    public Transform targetPoint;     // destination for rod
    public float moveDelay = 2f;      // animation delay before movement
    public float moveSpeed = 3f;      // rod move speed

    [Header("After rod reaches target")]
    public GameObject CubeRod;        // new object to unhide when rod finishes

    private bool shouldMove = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.LogError("[DoorRodManager] No Animator found on this GameObject.");
    }

    // Call this function to start animation + delayed rod movement
    public void PlayDoorAnimation()
    {
        if (anim == null) return;

        anim.Play("DoorAnim");
        Debug.Log("Door animation played.");

        CancelInvoke(nameof(StartRodMovement));
        Invoke(nameof(StartRodMovement), moveDelay);
    }

    private void StartRodMovement()
    {
        if (rod == null || targetPoint == null)
        {
            Debug.LogError("[DoorRodManager] Rod or targetPoint not assigned!");
            return;
        }
        shouldMove = true;
    }

    private void Update()
    {
        if (!shouldMove || rod == null || targetPoint == null) return;

        rod.transform.position = Vector3.MoveTowards(
            rod.transform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );

        // When rod reaches the destination
        if (Vector3.Distance(rod.transform.position, targetPoint.position) < 0.01f)
        {
            shouldMove = false;

            // 🔥 Hide rod
            rod.SetActive(false);

            // 🔥 Unhide CubeRod (if assigned)
            if (CubeRod != null)
                CubeRod.SetActive(true);

            Debug.Log("Rod reached target → Rod hidden, CubeRod shown.");
        }
    }
}
