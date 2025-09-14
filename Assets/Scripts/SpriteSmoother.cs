using UnityEngine;

public class SpriteSmoother : MonoBehaviour
{
    [SerializeField] private Transform spriteTransform; // Drag the SpriteHolder child here
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