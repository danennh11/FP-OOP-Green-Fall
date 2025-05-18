using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    private bool doubleJump;
    private bool isFacingRight = true;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing) return;

        // Check if player is still on ground or not
        if(IsGrounded() && !Input.GetKey(KeyCode.Space))
        {
            doubleJump = false;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(IsGrounded() || doubleJump)
            {
                Jump();
                doubleJump = !doubleJump; // 1st jump = true; 2nd jump = false
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        Move();
        BetterJump();
    }

    private void Move()
    {
        if (isDashing) return;

        float horizontalMov = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontalMov * 10, rb.velocity.y);

        // Flip sprite
        if (isFacingRight && horizontalMov < 0f || !isFacingRight && horizontalMov > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

        }
    }

    private void Jump()
    {
        if (isDashing) return;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
    }
    private void BetterJump()
    {
        if(rb.velocity.y < 0) //If player doesn't go up anymore
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f - 1) * Time.deltaTime;
        } else if(rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space)) // If player still go up but doesn't press the space btn
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2f - 1) * Time.deltaTime;
        }
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0);
        tr.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);

        canDash = true;
    }
}
