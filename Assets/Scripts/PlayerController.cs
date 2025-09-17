using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(GameObject))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float jumpForce = 10f;
    public float jumpCancelFactor;
    public float groundCheckRadius;
    public bool grounded;
    public bool isSliding;
    public bool canCancelJump;

    private BoxCollider2D boxCol;
    public Vector2 defaultSize;
    public Vector2 defaultOffset;
    [Header("Slide Collider Settings")]
    public Vector2 slideSize = new(1f, 0.5f);
    public Vector2 slideOffset = new(0f, -0.25f);
    public float slideTime = 0.8f;
    public float slideCooldown = 1f;
    private float slideEnd;

    // Temporary sprite objects for visualizing player height
    private SpriteRenderer tallHeight;
    private SpriteRenderer shortHeight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        slideEnd = Time.time - slideCooldown;

        // Temporary until we have animated sprites
        tallHeight = GameObject.Find("Tall").GetComponent<SpriteRenderer>();
        shortHeight = GameObject.Find("Short").GetComponent<SpriteRenderer>();
        tallHeight.enabled = true;
        shortHeight.enabled = false;
    }

    void Update()
    {
        CheckEnvironment();
        HandleJumpPress();
        HandleJumpCancel();
        HandleSlidePress();
    }

    void CheckEnvironment()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        //animator.SetBool("IsGrounded", grounded);
    }

    void HandleJumpPress()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        && grounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canCancelJump = true;

            //animator.SetBool("IsJumping", canCancelJump);
        }
    }

    void HandleJumpCancel()
    {
        if ((Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
        && canCancelJump
        && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCancelFactor);
            canCancelJump = false;
            //animator.SetBool("IsJumping", canCancelJump);
        }
        else if (canCancelJump && rb.linearVelocity.y < 0)
        {
            canCancelJump = false;
            //animator.SetBool("IsJumping", canCancelJump);
        }
    }

    void HandleSlidePress()
    {
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        && grounded
        && !isSliding
        && Time.time >= slideEnd + slideCooldown)
        {
            isSliding = true;
            boxCol.size = slideSize;
            boxCol.offset = slideOffset;

            StartCoroutine(SlideTimer());

            // Temporary until we have animated sprites
            tallHeight.enabled = false;
            shortHeight.enabled = true;
            //Debug.Log("Slide triggered!");
        }
    }

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

    void HandleSlideCancel()
    {
        slideEnd = Time.time;
        isSliding = false;
        boxCol.size = defaultSize;
        boxCol.offset = defaultOffset;

        // Temporary until we have animated sprites
        tallHeight.enabled = true;
        shortHeight.enabled = false;
        //Debug.Log("Slide canceled!");
    }

    // *** USED FOR DEV/DEBUG PURPOSES ***

    // void OnDrawGizmosSelected()
    // {
    //     if (groundCheck != null)
    //         Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    // }

    // void OnDrawGizmos()
    // {
    //     if (!boxCol) return;

    //     Gizmos.color = Color.green;
    //     DrawColliderGizmo(defaultSize, defaultOffset);   // standing

    //     Gizmos.color = Color.cyan;
    //     DrawColliderGizmo(slideSize, slideOffset);       // sliding
    // }

    // void DrawColliderGizmo(Vector2 size, Vector2 offset)
    // {
    //     // BoxCollider2D is centered around transform.position + offset
    //     Vector2 worldPos = (Vector2)transform.position + offset;

    //     Gizmos.matrix = Matrix4x4.TRS(worldPos, transform.rotation, Vector3.one);
    //     Gizmos.DrawWireCube(Vector3.zero, size);
    // }
}
