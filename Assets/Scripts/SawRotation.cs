using UnityEngine;

public class SawRotation : MonoBehaviour
{
    public float rotationSpeed = 100f;

    void Update()
    {
        // Rotate this object by increasing its z-axis value over time
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
