using UnityEngine;

public class StackChildrenAsSquare : MonoBehaviour
{
    public enum Orientation { HorizontalXZ, VerticalXY }
    [Header("Grid Orientation")]
    public Orientation orientation = Orientation.HorizontalXZ;

    [Header("Grid Settings")]
    [Tooltip("How many cubes in one row (X direction)")]
    public int columns = 5;

    [Tooltip("Arrange automatically in Start()")]
    public bool arrangeOnStart = true;

    [Tooltip("Extra gap between cubes (local units)")]
    public float extraGapX = 0f;
    public float extraGapYorZ = 0f; // used as Y gap in vertical mode or Z gap in horizontal mode

    [Tooltip("Center the grid around the parent origin")]
    public bool centerGrid = true;

    private void Start()
    {
        if (arrangeOnStart)
            ArrangeChildren();
    }

    [ContextMenu("Arrange Now")]
    public void ArrangeChildren()
    {
        int childCount = transform.childCount;
        if (childCount == 0 || columns <= 0)
            return;

        Transform firstChild = transform.GetChild(0);
        float sizeX = 1f;
        float sizeY = 1f;
        float sizeZ = 1f;

        // get actual renderer size
        Renderer r = firstChild.GetComponent<Renderer>();
        if (r != null)
        {
            Vector3 b = r.bounds.size;
            sizeX = b.x;
            sizeY = b.y;
            sizeZ = b.z;
        }
        else
        {
            Vector3 s = firstChild.localScale;
            sizeX = s.x;
            sizeY = s.y;
            sizeZ = s.z;
        }

        // step sizes based on orientation
        float stepX = sizeX + extraGapX;
        float stepYorZ = sizeY + extraGapYorZ; // Y in vertical, Z in horizontal
        if (orientation == Orientation.HorizontalXZ)
            stepYorZ = sizeZ + extraGapYorZ;

        int rows = Mathf.CeilToInt(childCount / (float)columns);

        // centering offset
        Vector3 originOffset = Vector3.zero;
        if (centerGrid)
        {
            float totalX = (columns - 1) * stepX;
            float totalYorZ = (rows - 1) * stepYorZ;

            if (orientation == Orientation.HorizontalXZ)
                originOffset = new Vector3(-totalX * 0.5f, 0, -totalYorZ * 0.5f);
            else
                originOffset = new Vector3(-totalX * 0.5f, -totalYorZ * 0.5f, 0);
        }

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            int col = i % columns;
            int row = i / columns;

            float x = col * stepX;
            float y = 0;
            float z = 0;

            if (orientation == Orientation.HorizontalXZ)
            {
                z = row * stepYorZ; // grid on ground plane
            }
            else
            {
                y = row * stepYorZ; // stacked vertically like shelves
            }

            child.localPosition = new Vector3(x, y, z) + originOffset;
            child.localRotation = Quaternion.identity;
        }
    }
}
