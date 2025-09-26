using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpriteSmoother : MonoBehaviour
{
    // Sprite should be detatched from Player object
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private float interpolationSpeed = 15f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = rb.position;
        spriteTransform.position = Vector3.Lerp(spriteTransform.position, targetPosition, interpolationSpeed * Time.deltaTime);
    }
}