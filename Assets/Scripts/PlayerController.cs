using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

// Remove the following line after adding animated sprites
[RequireComponent(typeof(GameObject))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject spriteHandle;
    [SerializeField] Animator animator;
    private bool isAlive = true;
    private bool isRunning = false;
    private bool canSlide = false;
    private bool canDive = false;

    private Rigidbody2D rb;
    [Header("Player Attributes")]
    public float jumpForce = 3.14f;
    public float jumpCancelFactor = -0.4f;
    public bool canCancelJump = false;
    public bool isSliding = false;
    [Header("Ground Checking")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public BoxCollider2D groundCollider;
    public float groundCheckRadius = 0.4f;
    public bool grounded;
    public float groundLevel;

    // Leniency for pressing jump early
    [Header("Jump Buffering")]
    public float groundProximityJumpFactor = 1f;
    public float jumpBufferTime = 0.25f;
    public bool bufferedJump = false;
    private float groundProximityDistance;
    private float bufferedJumpTimer = 0f;
    // Leniency for pressing slide early
    [Header("Slide Buffering")]
    public float groundProximitySlideFactor = 1f;
    public float slideBufferTime = 0.25f;
    public bool bufferedSlide = false;
    private float bufferedSlideTimer = 0f;

    private BoxCollider2D boxCol;
    [Header("Slide Collider Settings")]
    public Vector2 defaultSize;
    public Vector2 defaultOffset;
    public Vector2 slideSize = new(0.8f, 0.9f);
    public Vector2 slideOffset = new(0f, -0.05f);
    public float slideTime = 0.8f;
    public float slideCooldown = 0.4f;
    private float slideEnd;

    // Temporary sprite objects for visualizing player collider bounds
    private SpriteRenderer tallHeight;
    private SpriteRenderer shortHeight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();

        // Use the height of the player collider as the base ground proximity distance for the jump buffer
        groundProximityDistance = defaultSize.y;

        // groundLevel used to "snap" position to ground before applying a jump
        if (groundCollider != null)
        {
            groundLevel = groundCollider.bounds.max.y;

            // Notify groundLevel to SpawnManager
            Messenger<float>.Broadcast(GameEvent.SET_GROUND_HEIGHT, groundLevel);
        }

        // Initialize slide timer machanic
        slideEnd = Time.time - slideCooldown;

        animator.SetBool("IsRunning", isRunning);

        // Temporary until we have animated sprites
        tallHeight = GameObject.Find("Tall").GetComponent<SpriteRenderer>();
        shortHeight = GameObject.Find("Short").GetComponent<SpriteRenderer>();
        tallHeight.enabled = true;
        shortHeight.enabled = false;
    }

    void Update()
    {
        // Perform checks and read inputs while player is alive
        if (isAlive)
        {
            CheckEnvironment();
            HandleJumpPress();
            HandleJumpCancel();
            HandleSlideDivePress();
        }

        // Temporary, manually toggles the player collider visual on/off during a run
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (tallHeight.enabled || shortHeight.enabled)
            {
                tallHeight.enabled = false;
                shortHeight.enabled = false;
            }
            else
            {
                tallHeight.enabled = true;
                shortHeight.enabled = true;
            }
        }
    }

    void FixedUpdate()
    {
        // Debug ray, seen while in play mode
        Debug.DrawRay(transform.position, Vector2.down * groundProximityDistance, Color.red);
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.START_RUN, SetRunning);
        Messenger.AddListener(GameEvent.PLAYER_DIED, PlayerDied);
        Messenger<bool>.AddListener(GameEvent.SET_ABILITY_SLIDE, CanSlide);
        Messenger<bool>.AddListener(GameEvent.SET_ABILITY_DIVE, CanDive);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.START_RUN, SetRunning);
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, PlayerDied);
        Messenger<bool>.RemoveListener(GameEvent.SET_ABILITY_SLIDE, CanSlide);
        Messenger<bool>.RemoveListener(GameEvent.SET_ABILITY_DIVE, CanDive);
    }

    // Toggle running, -- from UIBracketMode
    void SetRunning()
    {
        if (isRunning) isRunning = false;
        else isRunning = true;

        animator.SetBool("IsRunning", isRunning);
    }

    // Allows slide ability, -- from GameManager
    void CanSlide(bool status)
    {
        canSlide = status;
    }

    // Allows dive ability, -- from GameManager
    void CanDive(bool status)
    {
        canDive = status;
    }

    // Slows this object to a stop when player dies, -- from PlayerHealth
    void PlayerDied()
    {
        // If player dies during upward momentum, cancel the momentum
        if (canCancelJump) ApplyJumpCancel();

        isRunning = false;
        isAlive = false;

        // Death sequence goes here

        animator.SetBool("IsRunning", isRunning);

        // Temporary, if player dies while sliding, restore original scale
        Vector3 scale = spriteHandle.transform.localScale;
        scale.y = 1.0f;
        spriteHandle.transform.localScale = scale;
    }

    // Perform environmental checks, apply buffers, verify slide input when slide buffer exists
    void CheckEnvironment()
    {
        // GROUND CHECK
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jump if landing with a buffered jump
        if (grounded && bufferedJump)
        {
            ApplyJump(groundLevel, jumpForce);
            bufferedJump = false;
            bufferedJumpTimer = 0f;
        }

        // Slide if landing with a buffered slide
        if (grounded && bufferedSlide)
        {
            // Apply the buffered slide only if key is held when contacting the ground
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !isSliding && Time.time >= slideEnd + slideCooldown)
            {
                BeginSlide();
            }
            bufferedSlide = false;
            bufferedSlideTimer = 0f;
        }

        // Update buffered jump timer and clear if expired
        if (bufferedJump)
        {
            bufferedJumpTimer += Time.deltaTime;
            if (bufferedJumpTimer >= jumpBufferTime)
            {
                bufferedJump = false;
                bufferedJumpTimer = 0f;
            }
        }

        // Update buffered slide timer and clear if expired
        if (bufferedSlide)
        {
            bufferedSlideTimer += Time.deltaTime;
            if (bufferedSlideTimer >= slideBufferTime)
            {
                bufferedSlide = false;
                bufferedSlideTimer = 0f;
            }
        }

        animator.SetBool("IsGrounded", grounded);
    }

    // Handle jump input
    void HandleJumpPress()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (grounded)
            {
                ApplyJump(groundLevel, jumpForce);
                return;
            }

            // If not grounded, check proximity to the ground and buffer the jump
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundProximityDistance * groundProximityJumpFactor, groundLayer);
            if (hit.collider != null)
            {
                bufferedJump = true;
                bufferedJumpTimer = 0f;
            }
        }
    }

    // Apply jump, -- from (self) or PlayerHealth
    public void ApplyJump(float baseHeight, float strength)
    {
        // Zero any vertical force before applying jump force
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        // Snap y-transform to jump base
        transform.position = new Vector2(transform.position.x, baseHeight);
        // Sqrt ensures max jump height is consistent
        rb.linearVelocityY = Mathf.Sqrt(2 * strength * Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale);

        canCancelJump = true;

        // Snap position of sprite handle to jump base only when jumping from ground level
        if (baseHeight == groundLevel) spriteHandle.transform.position = transform.position;
        animator.SetBool("IsJumping", canCancelJump);
    }

    // Handle jump cancel
    void HandleJumpCancel()
    {
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow)
        && canCancelJump
        && rb.linearVelocity.y > 0)
        {
            ApplyJumpCancel();
            canCancelJump = false;

            animator.SetBool("IsJumping", canCancelJump);
        }
        else if (canCancelJump && rb.linearVelocity.y < 0)
        {
            canCancelJump = false;

            animator.SetBool("IsJumping", canCancelJump);
        }
    }

    // Apply jump cancel
    void ApplyJumpCancel()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCancelFactor);
    }

    // Handle slide/dive input
    void HandleSlideDivePress()
    {
        // Slide if grounded and allowed
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && grounded && canSlide)
        {
            if (!isSliding && Time.time >= slideEnd + slideCooldown)
            {
                BeginSlide();
                return;
            }
        }

        // Decide whether to slide or dive, depending on context
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && !grounded && canSlide)
        {
            // Buffer slide if not grounded, allowed, and within buffer proximity
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundProximityDistance * groundProximitySlideFactor, groundLayer);
            if (hit.collider != null)
            {
                bufferedSlide = true;
                bufferedSlideTimer = 0f;
            }
            // Dive only if airborne and NOT within slide buffer proximity
            else if (canDive)
            {
                ApplyDive();
            }
        }
    }

    // Apply dive
    private void ApplyDive()
    {
        rb.linearVelocityY = -Mathf.Sqrt(4 * jumpForce * Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale);
    }

    // Begin slide
    private void BeginSlide()
    {
        isSliding = true;
        boxCol.size = slideSize;
        boxCol.offset = slideOffset;

        StartCoroutine(SlideTimer());

        // Temporary until we have animated sprites
        tallHeight.enabled = false;
        shortHeight.enabled = true;

        Vector3 scale = spriteHandle.transform.localScale;
        scale.y = 0.5f;
        spriteHandle.transform.localScale = scale;
    }

    // Limit slide duration
    private IEnumerator SlideTimer()
    {
        float elapsedTime = 0;

        // Slide as long as key is held and within timer limit
        while ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && elapsedTime < slideTime)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Cancel the slide if either requirement is broken
        HandleSlideCancel();
    }

    // Apply slide cancel
    void HandleSlideCancel()
    {
        slideEnd = Time.time;
        isSliding = false;
        boxCol.size = defaultSize;
        boxCol.offset = defaultOffset;

        // Temporary until we have animated sprites
        tallHeight.enabled = true;
        shortHeight.enabled = false;

        Vector3 scale = spriteHandle.transform.localScale;
        scale.y = 1.0f;
        spriteHandle.transform.localScale = scale;
    }

    // *** USED FOR DEV/DEBUG PURPOSES ***

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    void OnDrawGizmos()
    {
        if (!boxCol) return;

        Gizmos.color = Color.green;
        DrawColliderGizmo(defaultSize, defaultOffset);   // standing

        Gizmos.color = Color.cyan;
        DrawColliderGizmo(slideSize, slideOffset);       // sliding
    }

    void DrawColliderGizmo(Vector2 size, Vector2 offset)
    {
        // BoxCollider2D is centered around transform.position + offset
        Vector2 worldPos = (Vector2)transform.position + offset;

        Gizmos.matrix = Matrix4x4.TRS(worldPos, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}
