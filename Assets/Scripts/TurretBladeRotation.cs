using UnityEngine;

public class TurretBladeRotation : MonoBehaviour
{
    public float rotationSpeed = 100f;

    void Update()
    {
        // Rotate this object by increasing its x-axis value over time
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
    }
}
