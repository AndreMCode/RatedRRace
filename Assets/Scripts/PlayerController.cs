using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(GameObject))]
public class PlayerController : MonoBehaviour
{
    public InputAction jumpAction;
    public InputAction crouchAction;

    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] PlayerSFX playerSFX;

    [SerializeField] GameObject spriteHandle;
    [SerializeField] GameObject spriteObject;
    [SerializeField] Animator animator;
    private Vector3 spritePos;
    private Vector3 spriteScale;
    private float runAnimSpeed = 1;
    private bool isAlive = false;
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
    private bool grounded = true;
    private bool lastGroundState = true;
    private float groundLevel;

    [Header("Jump Buffering")]
    public float groundProximityJumpFactor = 1f;
    public float jumpBufferTime = 0.25f;
    public bool bufferedJump = false;
    private float groundProximityDistance;
    private float bufferedJumpTimer = 0f;

    private BoxCollider2D boxCol;

    [Header("Slide Collider Settings")]
    public Vector2 defaultSize;
    public Vector2 defaultOffset;
    public Vector2 slideSize = new(0.8f, 0.9f);
    public Vector2 slideOffset = new(0f, -0.05f);

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

        spritePos = spriteObject.transform.localPosition;
        spriteScale = spriteObject.transform.localScale;

        animator.SetBool("IsGrounded", true);
        animator.SetBool("IsRunning", false);
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
            HandleSlideCancel();
        }
    }

    void OnEnable()
    {
        jumpAction.Enable();
        crouchAction.Enable();
        Messenger.AddListener(GameEvent.PLAYER_READY, ReadyAnim);
        Messenger.AddListener(GameEvent.START_RUN, SetRunning);
        Messenger.AddListener(GameEvent.PLAYER_DIED, PlayerDied);
        Messenger<bool>.AddListener(GameEvent.SET_ABILITY_SLIDE, CanSlide);
        Messenger<bool>.AddListener(GameEvent.SET_ABILITY_DIVE, CanDive);
        Messenger<float>.AddListener(GameEvent.SET_RUN_SCALAR, SetRunAnimSpeed);
        Messenger.AddListener(GameEvent.PLAYER_TOGGLE_CONTROLS, ToggleControlsOnPause);
    }

    void OnDisable()
    {
        jumpAction.Disable();
        crouchAction.Disable();
        Messenger.RemoveListener(GameEvent.PLAYER_READY, ReadyAnim);
        Messenger.RemoveListener(GameEvent.START_RUN, SetRunning);
        Messenger.RemoveListener(GameEvent.PLAYER_DIED, PlayerDied);
        Messenger<bool>.RemoveListener(GameEvent.SET_ABILITY_SLIDE, CanSlide);
        Messenger<bool>.RemoveListener(GameEvent.SET_ABILITY_DIVE, CanDive);
        Messenger<float>.RemoveListener(GameEvent.SET_RUN_SCALAR, SetRunAnimSpeed);
        Messenger.RemoveListener(GameEvent.PLAYER_TOGGLE_CONTROLS, ToggleControlsOnPause);
    }

    // Perform environmental checks, apply buffers
    void CheckEnvironment()
    {
        // GROUND CHECK
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (!lastGroundState && grounded)
        {
            playerHealth.PlayerLanded();
            if (!isSliding) playerSFX.PlayRunSFX();

            animator.SetBool("IsLanding", false);
            animator.SetBool("IsDiving", false);

            animator.SetBool("IsGrounded", true);
            animator.SetBool("IsRunning", true);

            playerHealth.StopDiveEffectParticles();
            if (isSliding) playerHealth.StartSlideDustParticles();
        }
        lastGroundState = grounded;

        if (animator.GetBool("IsJumping") && rb.linearVelocityY < 0)
        {
            animator.SetBool("IsJumping", false);
            if (!isSliding)
            {
                animator.SetBool("IsLanding", true);
            }
        }

        // Jump if landing with a buffered jump
        if (grounded && bufferedJump)
        {
            ApplyJump(groundLevel, jumpForce);
            bufferedJump = false;
            bufferedJumpTimer = 0f;
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
    }

    // Handle jump input
    void HandleJumpPress()
    {
        if (isAlive && jumpAction.WasPressedThisFrame())
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
        playerSFX.StopRunSFX();
        playerSFX.PlayJumpSFX();
        canCancelJump = true;

        // Zero any vertical force before applying jump force
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        // Snap y-transform to jump base
        transform.position = new Vector2(transform.position.x, baseHeight);
        // Sqrt ensures max jump height is consistent
        rb.linearVelocityY = Mathf.Sqrt(2 * strength * Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale);
        // Snap position of sprite handle to jump base only when jumping from ground level
        if (baseHeight == groundLevel) spriteHandle.transform.position = transform.position;

        playerHealth.StopSlideDustParticles();
        playerHealth.StopDiveEffectParticles();

        animator.SetBool("IsRunning", false);
        animator.SetBool("IsLanding", false);
        animator.SetBool("IsDiving", false);
        animator.SetBool("IsGrounded", false);
        if (!isSliding) animator.SetBool("IsJumping", true);
    }

    // Handle jump cancel
    void HandleJumpCancel()
    {
        if (isAlive)
        {
            if (!jumpAction.IsPressed() && canCancelJump && rb.linearVelocity.y > 0)
            {
                ApplyJumpCancel();
                canCancelJump = false;
            }
            else if (canCancelJump && rb.linearVelocity.y < 0)
            {
                canCancelJump = false;
            }
        }
    }

    // Apply jump cancel
    void ApplyJumpCancel()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCancelFactor);

        if (!isSliding)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsLanding", true);
        }
    }

    // Handle slide/dive input
    void HandleSlideDivePress()
    {
        if (isAlive)
        {
            // Slide if grounded and allowed
            if (crouchAction.IsPressed() && grounded && canSlide && !isSliding)
            {
                BeginSlide();
                return;
            }

            // Decide whether to slide or dive, depending on context
            if (crouchAction.WasPressedThisFrame() && !grounded && canSlide)
            {
                if (bufferedJump) return;
                // Dive only if airborne
                else if (canDive)
                {
                    ApplyDive();
                }
            }
        }
    }

    // Begin slide
    private void BeginSlide()
    {
        playerSFX.StopRunSFX();
        playerSFX.StartSlideSFX();

        isSliding = true;
        boxCol.size = slideSize;
        boxCol.offset = slideOffset;

        Vector3 position = spriteObject.transform.position;
        position.y -= 0.4f;
        position.x += 0.15f;
        spriteObject.transform.position = position;

        Vector3 scale = spriteObject.transform.localScale;
        scale.x = 2.0f;
        spriteObject.transform.localScale = scale;

        playerHealth.ShrinkBubble();
        playerHealth.StartSlideDustParticles();

        animator.SetBool("IsRunning", false);
        animator.SetBool("IsSliding", true);
    }

    // Handle slide cancel
    void HandleSlideCancel()
    {
        if (isAlive)
        {
            // Cancel Slide on key up or shortly after mid-jump
            if ((crouchAction.WasReleasedThisFrame() && isSliding) || (!grounded && rb.linearVelocityY < -8.0f && isSliding))
            {
                ApplySlideCancel();
            }
        }
    }

    // Apply slide cancel
    void ApplySlideCancel()
    {
        playerSFX.StopSlideSFX();
        if (grounded) playerSFX.PlayRunSFX();

        isSliding = false;
        boxCol.size = defaultSize;
        boxCol.offset = defaultOffset;

        if (animator.GetBool("IsSliding"))
        {
            spriteObject.transform.localPosition = spritePos;
            spriteObject.transform.localScale = spriteScale;

            animator.SetBool("IsSliding", false);

            if (rb.linearVelocityY > 0)
            {
                animator.SetBool("IsJumping", true);
            }
            if (rb.linearVelocityY < 0)
            {
                animator.SetBool("IsLanding", true);
            }
        }
        playerHealth.RestoreBubble();
        playerHealth.StopSlideDustParticles();
    }

    // Apply dive
    private void ApplyDive()
    {
        rb.linearVelocityY = -Mathf.Sqrt(4 * jumpForce * Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale);

        playerHealth.StartDiveEffectParticles();
        playerSFX.PlayDiveSFX();

        animator.SetBool("IsJumping", false);
        animator.SetBool("IsLanding", false);
        animator.SetBool("IsDiving", true);
    }

    // Slows this object to a stop when player dies, -- from PlayerHealth
    void PlayerDied()
    {
        // If player dies during upward momentum, cancel the momentum
        if (canCancelJump) ApplyJumpCancel();

        isAlive = false;

        // Death sequence goes here

        animator.SetBool("IsRunning", false);
    }

    void ReadyAnim()
    {
        animator.SetBool("IsReady", true);
    }

    // Toggle running, -- from UIBracketMode
    void SetRunning()
    {
        isAlive = true;

        animator.SetBool("IsRunning", true);
        animator.SetBool("IsReady", false);

        playerSFX.PlayRunSFX();
    }

    void ToggleControlsOnPause()
    {
        isAlive = !isAlive;
        if (isSliding && isAlive && !crouchAction.IsPressed()) ApplySlideCancel();
        if (!isSliding && isAlive && grounded) playerSFX.PlayRunSFX();
    }

    void SetRunAnimSpeed(float scalar)
    {
        runAnimSpeed = scalar * 0.75f;
        animator.SetFloat("AnimSpeed", runAnimSpeed);
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

    // *** USED FOR DEV/DEBUG PURPOSES ***

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
