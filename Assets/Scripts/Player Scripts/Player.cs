using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private PlayerInputs input;
    private InputAction jumpAction, leftAction, rightAction;
    private Vector2 moveVector = Vector2.zero;
    private Vector2 moveDirection = Vector2.zero;
    public float moveSpeed = 5f;
    public float airSpeed = 4f;
    public float fallSpeed = 2f;
    public float jumpDeceleration = 0.5f;
    public float jumpForce = 25f; // The base jump force
    public bool facingRight = true;
    private bool inAir = false;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool movingLeft = false;
    [SerializeField] private bool movingRight = false;
    [SerializeField] private float coyoteTime = 0.1f; // Time in seconds to allow jumping after walking off a platform
    [SerializeField] private float coyoteTimer = 0f;
    private float defaultGravityScale = 5f;
    private float fallingGravityScale = 10f;
    private float initialSpeed;
    private float currentSpeed;

    void Awake()
    {
        input = new PlayerInputs();
        rigidBody = GetComponent<Rigidbody2D>();
        jumpAction = input.Player.Jump;
        leftAction = input.Player.Left;
        rightAction = input.Player.Right;
    }

    void FixedUpdate()
    {
        initialSpeed = 0f;
        currentSpeed = moveSpeed;

        // If Player is falling
        if (rigidBody.velocity.y < 0) {
            isJumping = false;
            rigidBody.gravityScale = fallingGravityScale;
        }

        if (inAir && !isJumping)
        {
            rigidBody.gravityScale = fallingGravityScale;
        }

        // If not holding A or D and in the Air, but still moving left or right
        if (!movingLeft && !movingRight && inAir && (rigidBody.velocity.x > 0 || rigidBody.velocity.x < 0)) {
            initialSpeed = moveSpeed;
            float t = Mathf.InverseLerp(1f,-fallSpeed, rigidBody.velocity.y);
            currentSpeed = Mathf.Lerp(fallSpeed, 0.5f, t);
            if (rigidBody.velocity.x > 0 )
                rigidBody.velocity = new Vector2(currentSpeed, rigidBody.velocity.y);
            if(rigidBody.velocity.x < 0) 
                rigidBody.velocity = new Vector2(-currentSpeed, rigidBody.velocity.y);
        }
        else {
            // Holding A on the ground
            if (movingLeft && !inAir)
            {
                rigidBody.position += Vector2.left * moveSpeed * Time.fixedDeltaTime;
            }
            // Holding A in the Air
            else if (movingLeft && inAir)
            {
                rigidBody.velocity = new Vector2(-airSpeed, rigidBody.velocity.y);
            }

            // Holding D on the ground
            if (movingRight && !inAir)
            {
                rigidBody.position += Vector2.right * moveSpeed * Time.fixedDeltaTime;
            }
            // Holding D in the Air
            else if (movingRight && inAir)
            {
                rigidBody.velocity = new Vector2(airSpeed, rigidBody.velocity.y);
            }

            // Coyote time countdown
            if (!isJumping && !inAir && rigidBody.velocity.y < 0) {
                coyoteTimer += Time.deltaTime;
            }
        }

        FlipCharacter();
    } // End Update

    private void OnEnable() 
    {
        jumpAction.Enable();
        leftAction.Enable();
        rightAction.Enable();
        jumpAction.performed += OnJumpPerformed;
        leftAction.performed += OnLeftPerformed;
        rightAction.performed += OnRightPerformed;

        inputs.Enable();
    }

    private void OnDisable()
    {
        jumpAction.Disable();
        leftAction.Disable();
        rightAction.Disable();
        jumpAction.performed -= OnJumpPerformed;
        leftAction.performed -= OnLeftPerformed;
        rightAction.performed -= OnRightPerformed;

        inputs.Disable();
    }

    private void Jump()
    {   
        // If the player still has time to jump
        if (coyoteTimer <= coyoteTime)
        {
            inAir = true;
            rigidBody.AddForce(new Vector2(rigidBody.velocity.x, jumpForce), ForceMode2D.Impulse);
        }
    }

    public void OnJumpPerformed(InputAction.CallbackContext context)
    { 
        if (context.started && !isJumping && !inAir) {
            isJumping = true;
            Jump();
        }
        if (context.canceled && isJumping) {
            isJumping = false;
        }
    }

    public void OnLeftPerformed(InputAction.CallbackContext context)
    {
        movingLeft = context.performed;
    }

    public void OnRightPerformed(InputAction.CallbackContext context)
    {
        movingRight = context.performed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            inAir = false;
            coyoteTimer = 0f;
            rigidBody.gravityScale = 5;
        }   
    }

    private void FlipCharacter()
    {
        // Check if the character is facing the opposite direction
        if (((facingRight && movingLeft) || (!facingRight && movingRight)) && !(movingLeft && movingRight))
        {
            // Flip the character by changing the x scale
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;

            // Update the facingRight variable
            facingRight = !facingRight;
        }
    }

} // end class
