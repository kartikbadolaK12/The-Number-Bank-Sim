using UnityEngine;

public class CubeStacksData : MonoBehaviour
{
    public static CubeStacksData Instance { get; private set; }

    [Header("Initial stack slots")]
    public Transform[] initialSlots;   // drag all initial slot transforms here

    [Header("Placed stack slots")]
    public Transform[] placedSlots;    // drag all placed slot transforms here

    public int InitialCount => initialSlots != null ? initialSlots.Length : 0;
    public int PlacedCount  => placedSlots  != null ? placedSlots.Length  : 0;

    private void Awake()
    {
        Instance = this;
    }
}
