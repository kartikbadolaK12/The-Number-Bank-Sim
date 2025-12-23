using System.Collections;
using UnityEngine;

public class VaultCubeCascade : MonoBehaviour
{
    [Header("Door Animation")]
    public Animator doorAnimator;
    public string doorAnimName = "DoorAnim"; // your animation state name
    public float doorDelay = 0.5f;           // delay BEFORE door animation starts
    public float cascadeDelay = 0.3f;        // delay AFTER door starts, before cubes

public AudioManager audioManager;
public AudioClip cubeCascadeSound;
    [Header("Cube Settings")]
    public GameObject cubePrefab;
    public Transform spawnCenter;
    public Vector3 spawnAreaSize = new Vector3(0.4f, 0.4f, 0.2f);
    public int cubeCount = 12;
    public float spawnInterval = 0.08f;

    [Header("Target (Table)")]
    public Transform tablePoint;

    [Header("Forces")]
    public float forwardForce = 2f;
    public float upwardForce = 2f;
    public float randomForce = 0.5f;

    private bool cascadePlayed = false;

    void Start()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // 1) Wait before starting door animation
        if (doorDelay > 0f)
            yield return new WaitForSeconds(doorDelay);

        // 2) Play door animation
        if (doorAnimator != null && !string.IsNullOrEmpty(doorAnimName))
        {
            doorAnimator.Play(doorAnimName, 0, 0f);
        }

        // 3) Wait before starting cube cascade
        if (cascadeDelay > 0f)
            yield return new WaitForSeconds(cascadeDelay);

        // 4) Spawn cubes
        PlayCascade();
    }

    public void PlayCascade()
    {
        if (cascadePlayed) return;
        cascadePlayed = true;
        audioManager.StopAudio();
        audioManager.PlayAudio(cubeCascadeSound);
        StartCoroutine(SpawnCubesRoutine());
    }

    IEnumerator SpawnCubesRoutine()
    {
        if (cubePrefab == null || spawnCenter == null)
        {
            Debug.LogWarning("VaultCubeCascade: cubePrefab or spawnCenter missing!");
            yield break;
        }

        for (int i = 0; i < cubeCount; i++)
        {
            SpawnOneCube();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOneCube()
    {
        Vector3 offset = new Vector3(
            Random.Range(-spawnAreaSize.x * 0.5f, spawnAreaSize.x * 0.5f),
            Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f),
            Random.Range(-spawnAreaSize.z * 0.5f, spawnAreaSize.z * 0.5f)
        );

        Vector3 spawnPos = spawnCenter.position + offset;
        GameObject cube = Instantiate(cubePrefab, spawnPos, Quaternion.identity);

        Rigidbody rb = cube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction;

            if (tablePoint != null)
                direction = (tablePoint.position - spawnPos).normalized; // aim at table
            else
                direction = transform.right; // fallback if table is on the right

            direction = direction * forwardForce + Vector3.up * upwardForce;
            direction += Random.insideUnitSphere * randomForce;

            rb.AddForce(direction, ForceMode.Impulse);
        }
    }
}
