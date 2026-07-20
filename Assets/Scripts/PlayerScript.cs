using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    private References refs;
    
    [SerializeField] private CapsuleCollider collider;
    [SerializeField] public Rigidbody rb;

    //Player input
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpInput;
    private bool shouldBeCrouched;
    
    //Player info
    public bool isOnGround;
    private bool isCrouched;
    private bool runAnimationIsOn;
    private float cameraXRotation;
    private float playerYRotation;

    private void Start()
    {
        refs = References.Refs;
    }

    private void FixedUpdate()
    {
        //The player can't move if the game isn't on
        if (!refs.gameLogic.gameIsOn)
            return;
        
        //Crouch and stand up
        Crouch();
        //Move forward, backward, left, right and jump
        Move();
        //Show the running animation if the player is running
        SetRunAnimation();
        //Gravity with scale
        Gravity();
    }
    
    private void Update()
    {
        //The player can't move if the game isn't on
        if (!refs.gameLogic.gameIsOn)
            return;
        
        //Look around with the mouse
        Look();
    }
    
    //--------
    //Movement
    //--------
    
    private void Crouch()
    {
        //If the player's crouch state is already correct, return
        if (shouldBeCrouched == isCrouched)
            return;

        var colliderHeight = refs.playerData.colliderHeight;
        
        if (shouldBeCrouched)
        {
            //Crouch
            isCrouched = true;
            refs.playerAnimator.SetBool("IsCrouched", true);
            
            //Make the collider shorter
            collider.height = colliderHeight / 2;
            collider.center = new Vector3(0, colliderHeight / 4, 0);
        }
        else
        {
            //Stand up
            isCrouched = false;
            refs.playerAnimator.SetBool("IsCrouched", false);
            
            //Make the collider taller
            collider.height = colliderHeight;
            collider.center = new Vector3(0, colliderHeight / 2, 0);
        }
    }

    private void Move()
    {
        //Get a movement vector from the direction the player is facing and the move input
        var movement = transform.forward * moveInput.y + transform.right * moveInput.x;
        
        //Change the max move speed based on if the player is crouched or not
        var maxMoveSpeed = isCrouched ? refs.playerData.maxCrouchSpeed : refs.playerData.maxRunSpeed;
        //Change the acceleration speed based on if the player is in the air or not
        var movementLerpStep = isOnGround ? refs.playerData.movementLerpStepGround : refs.playerData.movementLerpStepAir;
        
        //Use lerp functions to keep accelerating until a max speed
        var newXVelocity = Mathf.Lerp(
            rb.linearVelocity.x, movement.x * maxMoveSpeed, movementLerpStep);
        var newZVelocity = Mathf.Lerp(
            rb.linearVelocity.z, movement.z * maxMoveSpeed, movementLerpStep);
        
        //Set the velocities to 0 if they are very close to 0 (stop moving if the player is basically still)
        if (Mathf.Abs(newXVelocity) < 0.001f) newXVelocity = 0;
        if (Mathf.Abs(newZVelocity) < 0.001f) newZVelocity = 0;
        
        //Upwards velocity (jumping)
        var newYVelocity = jumpInput && isOnGround ? refs.playerData.jumpStrength : rb.linearVelocity.y;

        //Apply the velocity to the rigidbody of the player
        rb.linearVelocity = new Vector3(newXVelocity, newYVelocity, newZVelocity);
    }

    private void Look()
    {
        //Return if the player didn't move the mouse
        if (lookInput is { x: 0, y: 0 })
            return;
        
        //Get how much up/down and left/right the mouse moved
        var realSensitivity = Settings.Player.Sensitivity / 250;
        var mouseX = lookInput.x * realSensitivity;
        var mouseY = lookInput.y * realSensitivity;
        
        //Rotate the camera up and down locally
        cameraXRotation -= mouseY;
        cameraXRotation = Mathf.Clamp(cameraXRotation, -90, 90);
        refs.camera.transform.localRotation = Quaternion.Euler(cameraXRotation, 0, 0);
        
        //Rotate the player right and left
        playerYRotation += mouseX;
        transform.rotation = Quaternion.Euler(0, playerYRotation, 0);
    }

    private void SetRunAnimation()
    {
        //The speed at which the player is moving
        var speed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;
        //The running animation can be enabled if the player is moving quick enough
        var showRunAnimation = speed > refs.playerData.speedForRunAnimation && isOnGround;
        //Enable or disable the running animation if it's not already in the correct state
        if (showRunAnimation != runAnimationIsOn)
        {
            refs.playerAnimator.SetBool("IsRunning", showRunAnimation);
            runAnimationIsOn = showRunAnimation;
        }
    }

    private void Gravity()
    {
        //Apply gravity with a scale to the player
        if (!isOnGround)
            rb.AddForce(9.81f * refs.playerData.gravityScale * Vector3.down, ForceMode.Acceleration);
    }
    
    
    //------------
    //Player Input
    //------------
    
    public void OnMove(InputAction.CallbackContext context)
    {
        //Vector showing the player's WASD input
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //Vector showing the player's mouse movement
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        //Shoot if the player left-clicked and the game is on
        if (context.performed && refs.gameLogic.gameIsOn)
            refs.gun.Fire();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //When space is held down, jumpInput = true
        if (context.performed)
            jumpInput = true;
        else if (context.canceled)
            jumpInput = false;
    }
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (Settings.Player.ToggleCrouch)
        {
            //If toggle crouch, change state when the crouch button is pressed
            if (context.performed)
                shouldBeCrouched = !shouldBeCrouched;
        }
        else
        {
            //If not toggle crouch, crouch when pressed and uncrouch when let go
            if (context.performed)
                shouldBeCrouched = true;
            else if (context.canceled)
                shouldBeCrouched = false;
        }
    }
    
    //-----
    //Other
    //-----

    public void ResetPlayer()
    {
        //Move the player back to the start position
        rb.position = new Vector3(0, 0, -6);
        transform.position = new Vector3(0, 0, -6);
        refs.camera.transform.rotation = Quaternion.identity;
    }
}
