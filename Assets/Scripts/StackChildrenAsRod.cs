using UnityEngine;

public class StackChildrenAsRod : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Rod Settings")]
    public Axis stackAxis = Axis.Y;      // Direction of the rod
    public bool arrangeOnStart = true;   // Auto arrange in Start()

    [Tooltip("Extra gap between cubes (in local units)")]
    public float extraGap = 0f;

    private void Start()
    {
        if (arrangeOnStart)
            ArrangeChildren();
    }

    [ContextMenu("Arrange Now")]
    public void ArrangeChildren()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        // Use first child's renderer or localScale as size reference
        Transform firstChild = transform.GetChild(0);

        float size = 1f;

        // Try renderer bounds first (better with scaled objects)
        Renderer r = firstChild.GetComponent<Renderer>();
        if (r != null)
        {
            Vector3 boundsSize = r.bounds.size;
            if (stackAxis == Axis.X) size = boundsSize.x;
            else if (stackAxis == Axis.Y) size = boundsSize.y;
            else size = boundsSize.z;
        }
        else
        {
            // Fallback to localScale if no renderer
            Vector3 s = firstChild.localScale;
            if (stackAxis == Axis.X) size = s.x;
            else if (stackAxis == Axis.Y) size = s.y;
            else size = s.z;
        }

        // Step distance between each cube center
        float step = size + extraGap;

        // Arrange all children in a straight line in local space
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            float offset = step * i;

            Vector3 localPos = Vector3.zero;
            switch (stackAxis)
            {
                case Axis.X:
                    localPos = new Vector3(offset, 0f, 0f);
                    break;
                case Axis.Y:
                    localPos = new Vector3(0f, offset, 0f);
                    break;
                case Axis.Z:
                    localPos = new Vector3(0f, 0f, offset);
                    break;
            }

            child.localPosition = localPos;
            child.localRotation = Quaternion.identity;  // keep them aligned
        }
    }
}
