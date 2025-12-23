using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DragOnlyXY : MonoBehaviour
{
    [Header("Camera")]
    public Camera cam;   // Assign in Inspector. If left empty, uses Camera.main

    [Header("X Clamp (optional)")]
    public bool clampX = false;
    public float minX = -5f;
    public float maxX = 5f;

    [Header("Y Clamp (optional top limit)")]
    public bool clampTopY = false;
    public float maxY = 3f;

    [Header("Global Bottom Y Limit (world space)")]
    public float minWorldY = -0.2560219f;

    [Header("Delay before drag is enabled (seconds)")]
    public float dragEnableDelay = 0f;   // 👈 public delay

    private bool dragging = false;
    private float distanceToCamera;
    private float fixedZ;
    private Vector3 grabOffset;

    // internal timer
    private bool canDrag = false;
    private float dragTimer = 0f;

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        // setup delay
        dragTimer = dragEnableDelay;
        canDrag = (dragEnableDelay <= 0f);
    }

    void Update()
    {
        if (cam == null) return;

        // 🔒 Handle global delay before drag is allowed
        if (!canDrag)
        {
            dragTimer -= Time.deltaTime;
            if (dragTimer <= 0f)
                canDrag = true;

            return; // ❗ nothing else works until delay is over
        }

        Vector3 pointerPos;

        bool pointerDown = GetPointerDown(out pointerPos);
        bool pointerHeld = GetPointer(out pointerPos);
        bool pointerUp   = GetPointerUp();

        // ---------- Start drag ----------
        if (pointerDown)
        {
            Ray ray = cam.ScreenPointToRay(pointerPos);

            // get ALL hits so we can skip PlaceZone tag
            RaycastHit[] hits = Physics.RaycastAll(ray);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance)); // nearest first

            foreach (RaycastHit h in hits)
            {
                // Skip the collider tagged "PlaceZone"
                if (h.collider.CompareTag("PlaceZone"))
                    continue;

                // If the ray hits THIS cube, start dragging
                if (h.transform == transform)
                {
                    dragging = true;
                    distanceToCamera = h.distance;
                    fixedZ = transform.position.z;
                    grabOffset = transform.position - h.point;
                }

                // Stop checking after the first non-PlaceZone object
                break;
            }
        }

        // ---------- Dragging ----------
        if (dragging && pointerHeld)
        {
            Ray ray = cam.ScreenPointToRay(pointerPos);
            Vector3 hitPoint = ray.GetPoint(distanceToCamera);

            Vector3 targetPos = hitPoint + grabOffset;

            float x = targetPos.x;
            float y = targetPos.y;

            // X clamp (optional)
            if (clampX)
                x = Mathf.Clamp(x, minX, maxX);

            // Global bottom Y limit
            if (y < minWorldY)
                y = minWorldY;

            // Optional top clamp
            if (clampTopY && y > maxY)
                y = maxY;

            transform.position = new Vector3(x, y, fixedZ);
        }

        // ---------- End drag ----------
        if (pointerUp)
            dragging = false;
    }

    // ---------- Pointer helpers (mouse + touch) ----------

    bool GetPointerDown(out Vector3 pos)
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                pos = t.position;
                return true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            pos = Input.mousePosition;
            return true;
        }

        pos = default;
        return false;
    }

    bool GetPointer(out Vector3 pos)
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                pos = t.position;
                return true;
            }
        }

        if (Input.GetMouseButton(0))
        {
            pos = Input.mousePosition;
            return true;
        }

        pos = default;
        return false;
    }

    bool GetPointerUp()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                return true;
        }

        return Input.GetMouseButtonUp(0);
    }
}
