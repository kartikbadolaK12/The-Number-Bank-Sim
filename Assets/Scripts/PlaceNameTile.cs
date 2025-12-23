using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class PlaceNameTile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("What place is this tile?")]
    public PlaceType placeType;

    private Canvas canvas;
    private CanvasGroup cg;

    [HideInInspector] public Transform originalParent;
    [HideInInspector] public Vector3 originalPos;
    [HideInInspector] public Vector3 originalScale;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        cg = GetComponent<CanvasGroup>();

        originalParent = transform.parent;
        originalPos    = transform.localPosition;
        originalScale  = transform.localScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Store current state (in case layout changed)
        originalParent = transform.parent;
        originalPos    = transform.localPosition;
        originalScale  = transform.localScale;

        // 🔁 Back to your old behaviour – this was working fine
        transform.SetParent(canvas.transform, true);
        cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform rt = transform as RectTransform;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pos);

        rt.localPosition = pos;   // same as your original code
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cg.blocksRaycasts = true;

        // If no slot accepted it, we are still under canvas → go back
        if (transform.parent == canvas.transform)
        {
            ResetPosition();
        }
    }

    public void ResetPosition()
    {
        // ⬅ Back under original parent
        transform.SetParent(originalParent, false);

        // restore exact transform
        transform.localPosition = originalPos;
        transform.localScale    = originalScale;  // 👈 this fixes the "squeezed" problem
    }
}
