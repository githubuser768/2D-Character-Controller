using UnityEngine;

public class Character_Controller : MonoBehaviour
{
    private float horizontal;
    public float movespeed = 10f;
    public float jumpforce = 35f;
    private bool isFacingRight;

    private bool DoubleJump = true;

    private float coyotetime = 0.05f;
    private float coyotetimecounter;

    private float jumpBuffertime = 0.01f;
    private float jumpBuffercounter;

    private bool canDash = true;
    private bool isDashing;
    public float dashtime = 0.2f;
    public float dashpower = 30f;
    public float dashcooldown = 1f;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform groundcheck;
    [SerializeField] LayerMask groundlayer;
    [SerializeField] TrailRenderer tr;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (isDashing)
        {
            return;
        }


        if (IsGrounded())
        {
            coyotetimecounter = coyotetime;
        }
        else
        {
            coyotetimecounter -= Time.deltaTime;
        }


        if (Input.GetButtonDown("Jump"))
        {
            jumpBuffercounter = jumpBuffertime;
        }
        else
        {
            jumpBuffercounter -= Time.deltaTime;
        }


        if(IsGrounded() && Input.GetButton("Jump"))
        {
            DoubleJump = false;
        }


        if(jumpBuffercounter > 0f)
        {
            if(coyotetimecounter > 0f || DoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpforce);
                DoubleJump = !DoubleJump;
            }
        }


        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyotetimecounter = 0f;
            jumpBuffercounter = 0f;
        }


        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }


        Flip();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        rb.velocity = new Vector2(horizontal * movespeed, rb.velocity.y);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal > 0f || !isFacingRight && horizontal < 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localscale = transform.localScale;
            localscale.x *= -1f;
            transform.localScale = localscale;
        }
        
    }


    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundcheck.position, 0.2f, groundlayer);
    }

    private System.Collections.IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalgravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(rb.velocity.x * dashpower, rb.velocity.y);
        tr.emitting = true;
        yield return new WaitForSeconds(dashtime);
        tr.emitting = false;
        rb.gravityScale = originalgravity;
        isDashing = false;
        yield return new WaitForSeconds(dashcooldown);
        canDash = true;
    }
}
