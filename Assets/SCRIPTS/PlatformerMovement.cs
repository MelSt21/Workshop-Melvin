using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

            /////////////// INFORMATION ///////////////
// This script automatically adds a Rigidbody2D, CapsuleCollider2D and CircleCollider2D component in the inspector.
// The Rigidbody2D component should (probably) have some constraints: Freeze Rotation Z
// The Circle Collider 2D should be set to "is trigger", resized and moved to a proper position.

// The following components are also needed: Player Input
// Gravity for the project is set in Unity at Edit -> Project Settings -> Physics2D -> Gravity Y

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(CapsuleCollider2D))]
public class PlatformerMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    // [SerializeField] private float gravityMultiplier = 1;    //unused
    [SerializeField] private SpriteRenderer spriteRenderer;

    public bool controlEnabled { get; set; } = true; // You can edit this variable from Unity Events
    public UnityEvent onAction2;
    
    private Vector2 moveInput;
    private Rigidbody2D rb;
    
    // Platformer specific variables
    private CircleCollider2D groundCheckCollider;
    private LayerMask groundLayer = ~0; // ~0 is referring to EVERY layer. Do you want a specific layer? Serialize the variable and assign the Layer of your choice.
    private Vector2 velocity;
    private bool jumpInput;
    private bool jumpReleased;
    private bool wasGrounded;
    private bool isGrounded;

    [SerializeField] private Animator animator;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        groundCheckCollider = GetComponent<CircleCollider2D>();
        groundCheckCollider.isTrigger = true;
        
        // Set gravity scale to 0 so player won't "fall" 
        rb.gravityScale = 0;

        //animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        velocity = TranslateInputToVelocity(moveInput);
        
        // Apply jump-input:
        if (jumpInput && wasGrounded)
        {
            velocity.y = jumpForce;
            jumpInput = false;
        }
        
        // Check if character lost contact with ground this frame
        if (wasGrounded == true && isGrounded == false)
        {
            if (velocity.y < 0)
            {
                // Has fallen. Play fall sound and/or trigger fall animation etc
            }
            else
            {
                // Has jumped. Play jump sound and/or trigger jump animation etc
            }
        }
        // Check if character gained contact with ground this frame
        else if (wasGrounded == false && isGrounded == true)
        {
            jumpReleased = false;
        }
        wasGrounded = isGrounded;
        
        // Flip sprite according to direction (if a sprite renderer has been assigned)
        if (spriteRenderer)
        {
            if (moveInput.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (moveInput.x < -0.01f)
                spriteRenderer.flipX = true;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
        ApplyGravity();
        rb.velocity = velocity;
        
        // Write movement animation code here. (Suggestion: send your current velocity into the Animator for both the x- and y-axis.)
    }

    private bool IsGrounded()
    {
        // Is our groundCheckCollider touching the groundLayer? If so, return the value "true"
        if (groundCheckCollider.IsTouchingLayers(groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ApplyGravity()
    {
        // Applies a set gravity for when player is grounded
        if (isGrounded && velocity.y < 0.0f)
        {
            velocity.y = -1.0f;
        }
        // Updates fall speed with gravity if object isn't grounded
        else
        {
            // Is Jumping upwards
            if (velocity.y > 0)
            {
                float deceleration = 1;
                if (jumpReleased) // shorter jump height when releasing the jump-key
                {
                    deceleration = 5;
                }
                // you can add a gravity multiplier here... but how?
                velocity.y += Physics2D.gravity.y * deceleration * Time.deltaTime;
            }
            // Is Falling
            else
            {
                // you can add a gravity multiplier here... but how?
                velocity.y += Physics2D.gravity.y * Time.deltaTime;
            }
        }
    }
    
    Vector2 TranslateInputToVelocity(Vector2 input)
    {
        // Make the character move along the X-axis
        return new Vector2(input.x * maxSpeed, velocity.y);
    }

    // Handle Move-input
    // This method can be triggered through the UnityEvent in PlayerInput
    public void OnMove(InputAction.CallbackContext context)
    {
        if (controlEnabled)
        {
            moveInput = context.ReadValue<Vector2>().normalized;
        }
        else
        {
            moveInput = Vector2.zero;
        }
    }

    // Handle Jump-input
    // This method can be triggered through the UnityEvent in PlayerInput
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && controlEnabled)
        {
            Debug.Log("Jump!");
            jumpInput = true;
            jumpReleased = false;
        }

        if (context.canceled && controlEnabled)
        {
            jumpReleased = true;
            jumpInput = false;
        }
    }

    public void Action2(InputAction.CallbackContext context)
    {
        if (context.started && controlEnabled)
        {
            Debug.Log("Action2!");
            onAction2.Invoke();
        }
    }
}
