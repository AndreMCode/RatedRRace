using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCol;
    private readonly int baseHealth = 1;
    private int health;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        // Apply powerup modifiers (bubble) in GameManager
    }

    void Update()
    {
        if (health == 0)
        {
            // End of run calls, etc:
            Messenger.Broadcast(GameEvent.PLAYER_DIED);

            health -= 1; // prevent further calls
        }
    }

    void OnEnable()
    {
        Messenger<int>.AddListener(GameEvent.SET_HEALTH, SetHealth);
    }

    void OnDisable()
    {
        Messenger<int>.RemoveListener(GameEvent.SET_HEALTH, SetHealth);
    }

    void SetHealth(int defense)
    {
        health = baseHealth + defense;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            if (boxCol.bounds.min.y >= collision.transform.position.y && rb.linearVelocityY < 0f)
            {
                // Player is above box, destroy box
                Debug.Log("Destroyed box from above!");
                Destroy(collision.gameObject);
            }
            else
            {
                // Player hit box from side or below, lose health
                Debug.Log("Crashed into box from the side!");
                Destroy(collision.gameObject);
                health -= 1;
            }

            return;
        }

        if (collision.gameObject.CompareTag("Saw"))
        {
            Debug.Log("Shredded by saw object!");
            Destroy(collision.gameObject);
            health -= 1;

            return;
        }
    }
}
