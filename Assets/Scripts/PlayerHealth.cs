using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PlayerController))]

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] GameObject spriteHandle;
    private Rigidbody2D rb;
    private BoxCollider2D boxCol;
    private PlayerController playerController;
    private readonly int baseHealth = 1;
    private int health;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        playerController = GetComponent<PlayerController>();
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
            BoxCollider2D box = collision.GetComponent<BoxCollider2D>();

            // Since the sprite handle position on approach is always some distance behind the rigidbody
            // (it's being lerped for smooth framerate), its position serves as a check in cases when
            // the rigidbody is detected after passing through a collider by continuous collision detection
            if (box != null && spriteHandle.transform.position.y > box.bounds.max.y)
            {
                float baseHeight = collision.GetComponent<BoxCollider2D>().bounds.max.y;
                playerController.ApplyJump(baseHeight, playerController.jumpForce / 2);

                // Player is above box, destroy box
                // Debug.Log("Destroyed box from above! PlayerCol min: " + boxCol.bounds.min.y + " Box min: " + box.bounds.min.y + " Box max: " + box.bounds.max.y + " Box cutoff point: " + cutoffPoint);
                Destroy(collision.gameObject);
            }
            else
            {
                // Player hit box from side or below, lose health
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
