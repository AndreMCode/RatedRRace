using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float jumpForce = 10f;
    public float jumpCancelFactor;
    public float groundCheckRadius;
    public bool grounded;
    public bool canCancelJump;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckEnvironment();
        HandleJumpPress();
        HandleJumpCancel();
    }

    void CheckEnvironment()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        // animator.SetBool("IsGrounded", grounded);
    }

    void HandleJumpPress()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if (grounded)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                canCancelJump = true;

                // animator.SetBool("IsJumping", canCancelJump);
            }
        }
    }

    void HandleJumpCancel()
    {
        if ((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.JoystickButton0))
            && canCancelJump && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCancelFactor);
            canCancelJump = false;
            // animator.SetBool("IsJumping", canCancelJump);
        }
        else if (canCancelJump && rb.linearVelocity.y < 0)
        {
            canCancelJump = false;
            // animator.SetBool("IsJumping", canCancelJump);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
