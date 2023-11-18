using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[CreateAssetMenu(fileName = "PlayerController", menuName = "InputController/PlayerController")]
public class PlayerController : MonoBehaviour
{
    private PlayerInputs input;
    private InputAction jumpAction, leftAction, rightAction;
    protected bool isJumping = false;
    protected bool inAir = false;
    protected bool movingLeft = false;
    protected bool movingRight = false;

    void Awake()
    {
        input = new PlayerInputs();
        jumpAction = input.Player.Jump;
        leftAction = input.Player.Left;
        rightAction = input.Player.Right;
    }

    private void OnEnable() 
    {
        if (jumpAction != null)
        {
            jumpAction.Enable();
            jumpAction.performed += OnJumpPerformed;
        }

        if (leftAction != null)
        {
            leftAction.Enable(); 
            leftAction.performed += OnLeftPerformed;
        }

        if (rightAction != null)
        {
            rightAction.Enable();
            rightAction.performed += OnRightPerformed;
        }
    }

    private void OnDisable()
    {
        if (jumpAction != null)
        {
            jumpAction.Disable();
            jumpAction.performed -= OnJumpPerformed;
        }

        if (leftAction != null)
        {
            leftAction.Disable(); 
            leftAction.performed -= OnLeftPerformed;
        }

        if (rightAction != null)
        {
            rightAction.Disable();
            rightAction.performed -= OnRightPerformed;
        }
    }

    public void OnJumpPerformed(InputAction.CallbackContext context)
    { 
        // if (context.started && !isJumping && !inAir) {
        //     isJumping = true;
        //     //Jump();
        // }
        // if (context.canceled && isJumping) {
        //     isJumping = false;
        // }
        isJumping = context.performed;
    }

    public void OnLeftPerformed(InputAction.CallbackContext context)
    {
        movingLeft = context.performed;
    }

    public void OnRightPerformed(InputAction.CallbackContext context)
    {
        movingRight = context.performed;
    }
}
