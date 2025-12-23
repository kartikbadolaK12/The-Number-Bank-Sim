using UnityEngine;

public class SpinText : MonoBehaviour
{
    public float rotationSpeed = 50f;

    void Update()
    {
        // Rotate around Z axis (spin like a sign)
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
