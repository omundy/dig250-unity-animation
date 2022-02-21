using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Take input from player
 */

public class PlayerMovement : MonoBehaviour
{
    // references to components
    public CharacterController2D controller;
    public Animator animator;

    // horizontal movement speed and direction
    public float runSpeed = 40f;
    public float horizontalMove = 0f;

    // boolean to detect jump input
    [SerializeField] bool jump = false;
    [SerializeField] bool crouch = false;

    // use Update to get input from player 
    void Update()
    {
        // get horizontal input (between -1 and 1) from player
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        //Debug.Log("horizontalMove = " + horizontalMove);

        // set the speed variable
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        // if jump button pressed
        if (Input.GetButtonDown("Jump"))
        {
            // set true and show animation 
            jump = true;
            animator.SetBool("IsJumping", true);
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }
    }

    // use FixedUpdate to move character so position is in sync with physics calculations 
    private void FixedUpdate()
    {
        // move the character using horizontal movement * amount of time elapsed since last frame rendered
        // this ensures the movement is the same across different computers, regardless of the frame rate and platform
        // 2nd/3rd parameters are for crouch and jump booleans
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);

        // reset jump
        jump = false;
    }

    // called from character controller
    public void OnLanding()
    {
        // stop jump animation when characte lands
        animator.SetBool("IsJumping", false);
    }
    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("IsCrouching", isCrouching);
    }


}