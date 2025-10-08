using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PlayerController))]

public class PlayerHealth : MonoBehaviour
{
    // Manages player defenses and collision actions
    // ---------------------------------------------

    [SerializeField] GameObject spriteHandle;
    private PlayerController playerController;
    private readonly int baseHealth = 1;
    private int health;
    private bool isAlive = true;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // Check if player lost all defenses
        if (isAlive && health <= 0)
        {
            Messenger.Broadcast(GameEvent.PLAYER_DIED);

            isAlive = false;
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

    // Set defense quantity (1 hit plus Bubble Shield layers), -- from GameManager
    void SetHealth(int defense)
    {
        health = baseHealth + defense;
    }

    // Decide what to do when colliding with an obstacle
    void OnTriggerEnter2D(Collider2D collision)
    {
        // If we collide with a Box..
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
                Destroy(collision.gameObject);
            }
            else
            {
                // Player hit box from side or below, lose defense
                Destroy(collision.gameObject);
                health -= 1;
            }

            return;
        }

        // If we collide with a Saw..
        if (collision.gameObject.CompareTag("Saw"))
        {
            // Player sliced up by Saw, lose defense
            Destroy(collision.gameObject);
            health -= 1;

            return;
        }
    }
}
