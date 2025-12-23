using UnityEngine;

public class ActivateGrag : MonoBehaviour
{
    [Header("Delay before enabling drag")]
    public float delay = 1.5f;   // you can edit from Inspector

    private PlaceNameTile tile;

    void Start()
    {
        tile = GetComponent<PlaceNameTile>();
        if (tile != null)
            tile.enabled = false;      // disable first

        StartCoroutine(EnableAfterDelay());
    }

    private System.Collections.IEnumerator EnableAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (tile != null)
            tile.enabled = true;       // enable after delay
    }
}
