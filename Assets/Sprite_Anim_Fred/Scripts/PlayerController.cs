using System;
using UnityEngine;

/// Basic 2D platformer character controller
/// 2022 Owen Mundy
/// Based on Brackeys "2D Movement in Unity (Tutorial)"
/// https://www.youtube.com/watch?v=dwcT-Dch0bA&list=PLPV2KyIb3jR6TFcFuzI2bB7TMNIIBpKMQ&index=2&t=1008s


// Ensure required components are also added to Game Object
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]

    // horizontal movement speed and direction
    public Vector2 playerInput;
	
    // boolean to detect / test jump input
    [SerializeField] bool jumpPress = false;
    [SerializeField] bool jumpHold = false;


    [Header("Parameters")]

    [SerializeField] private LayerMask groundLayer; // A mask determining what is ground to the character
    [SerializeField] private Transform ceilingCheck; // A position marking where to check for ceilings
    [SerializeField] private Transform groundCheck; // A position marking where to check if the player is isGrounded.

    private Vector3 velocity = Vector3.zero;
    public float runSpeed = 30f;
    [SerializeField] private float jumpForce = 6.2f; // Force added when the player jumps
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 3f;

    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f; // How much to smooth out the movement
    [SerializeField] private bool airControl = true; // Whether or not a player can steer while jumping

    const float isGroundedRadius = .2f; // Radius of the overlap circle to determine if isGrounded

    public float rememberGroundedFor = 0.1f;
    float lastTimeGrounded;

    private Rigidbody2D rb2d;


    [Header("Display")]

    public Animator animator;
    [SerializeField] bool facingRight = true;  // For determining which way the player is currently facing.
    public bool isGrounded; // is the player on the ground?


    private void Awake()
    {
        // get components, stop game if not found
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (!rb2d || !animator)
        {
            Debug.LogError("Rigidbody2D and Animator required");
            UnityEditor.EditorApplication.isPlaying = false;
        }
		
        // get child objects, stop game if not found
        groundCheck = transform.Find("GroundCheck").gameObject.transform;
        ceilingCheck = transform.Find("CeilingCheck").gameObject.transform;
        if (!groundCheck || !ceilingCheck)
        {
            Debug.LogError("GroundCheck and CeilingCheck required");
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    private void Update()
    {
        // get horizontal input (between -1 and 1) from player
        playerInput.x = Input.GetAxisRaw("Horizontal");
        Move(playerInput.x * runSpeed * Time.fixedDeltaTime);

        // check if jump keys / buttons are pressed on this loop
        jumpPress = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow);
        Jump();
        // check if jump keys / buttons are held down
        jumpHold = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow);
        BetterJump();

        // whether currently on ground (collision detection)
        CheckIfGrounded();
    }

    public void Move(float move)
    {
        // set the speed variable
        animator.SetFloat("Speed", Mathf.Abs(move));

        //only control the player if isGrounded or airControl is turned on
        if (isGrounded || airControl)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, rb2d.linearVelocity.y);
            // And then smoothing it out and applying it to the character
            rb2d.linearVelocity = Vector3.SmoothDamp(rb2d.linearVelocity, targetVelocity, ref velocity, movementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if ((move > 0 && !facingRight) || (move < 0 && facingRight))
            {
                Flip();
            }
        }
    }

    public void Jump()
    {
        // if jump button pressed + either grounded or late jump off edge 
        if (jumpPress && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor))
        {
            // Add a vertical force to the player 
            // => Brackeys version: e.g. jumpForce = 550, rb2d.gravityScale = 3
            //rb2d.AddForce(new Vector2(0f, jumpForce));

            // Add a vertical force to the player 
            // => craftgames version: e.g. jumpForce = 6, rb2d.gravityScale = 1
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);

            // reset for late jump
            lastTimeGrounded = Time.time;
            animator.SetBool("IsJumping", true);
        }
    }

    // adjust falling and jumping speed based on whether player still has jump pressed
    // for more see: https://www.youtube.com/watch?v=7KiK0Aqtmzc
    void BetterJump()
    {
        // slow down velocity for all
        if (rb2d.linearVelocity.y < 0)
        {
            rb2d.linearVelocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        // slow up velocity if they release early
        else if (rb2d.linearVelocity.y > 0 && !jumpHold)
        {
            rb2d.linearVelocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;
        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void CheckIfGrounded()
    {
        // is currently grounded?
        bool wasGrounded = isGrounded;
        isGrounded = false;

        // Check if a circle cast to the groundcheck position hits anything designated as Ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, isGroundedRadius, groundLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                // if not parent collider then set true 
                isGrounded = true;
                // if wasn't already on ground then it just landed
                if (!wasGrounded)
                {
                    //Debug.Log("collided with: " + colliders[i].gameObject.name);
                    OnLanding();
                }
            }
        }
    }

    void OnLanding()
    {
        //Debug.Log("OnLanding");
        animator.SetBool("IsJumping", false);
    }
}