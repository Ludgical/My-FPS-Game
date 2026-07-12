using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private GunScript gun;
    [SerializeField] private GameObject gunPosition;
    [SerializeField] private Animator animator;
    [SerializeField] private CapsuleCollider collider;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LogicScript logic;
    [SerializeField] private DoorScript door;

    //Player input
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpInput;
    private bool shouldBeCrouched;
    
    //Player info
    private bool isOnGround;
    private bool isCrouched;
    private bool runAnimationIsOn;
    private float cameraXRotation;
    private float playerYRotation;
    
    private void Start()
    {
        SetFOV(logic.fov);
    }

    private void SetFOV(float newFov)
    {
        camera.fieldOfView = newFov;
        gun.SetZPosition(logic.fov);
    }

    public void SetIsOnGround(bool value)
    {
        isOnGround = value;
    }
    
    private void FixedUpdate()
    {
        //Move to when the fov slider changes (temporary)
        // SetFOV(logic.fov);
        
        if (!logic.gameIsOn)
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
        if (!logic.gameIsOn)
            return;
        
        //Look around with the mouse
        Look();
    }
    
    //--------
    //Movement
    //--------
    
    private void Crouch()
    {
        if (shouldBeCrouched == isCrouched)
            return;

        var colliderHeight = logic.colliderHeight;
        
        if (shouldBeCrouched)
        {
            //Crouch
            isCrouched = true;
            animator.SetBool("IsCrouched", true);
            
            //Make the collider shorter
            collider.height = colliderHeight / 2;
            collider.center = new Vector3(0, colliderHeight / 4, 0);
        }
        else
        {
            //Stand up
            isCrouched = false;
            animator.SetBool("IsCrouched", false);
            
            //Make the collider taller
            collider.height = colliderHeight;
            collider.center = new Vector3(0, colliderHeight / 2, 0);
        }
    }

    private void Move()
    {
        //Get a movement vector from the direction the player is facing and the move input
        var movement = transform.forward * moveInput.y + transform.right * moveInput.x;
        
        //Use lerp functions to keep accelerating until a max speed
        var maxMoveSpeed = isCrouched ? logic.maxCrouchSpeed : logic.maxRunSpeed;
        var movementLerpStep = isOnGround ? logic.movementLerpStepGround : logic.movementLerpStepAir;
        
        var newXVelocity = Mathf.Lerp(
            rb.linearVelocity.x, movement.x * maxMoveSpeed, movementLerpStep);
        var newZVelocity = Mathf.Lerp(
            rb.linearVelocity.z, movement.z * maxMoveSpeed, movementLerpStep);
        
        //Set the velocities to 0 if they are very close to 0 (stop moving if the player is basically still)
        newXVelocity = newXVelocity is > -0.001f and < 0.001f ? 0 : newXVelocity;
        newZVelocity = newZVelocity is > -0.001f and < 0.001f ? 0 : newZVelocity;
        
        //Upwards velocity (jumping)
        var newYVelocity = jumpInput && isOnGround ? logic.jumpStrength : rb.linearVelocity.y;

        //Apply the velocity to the rigidbody of the player
        rb.linearVelocity = new Vector3(newXVelocity, newYVelocity, newZVelocity);
    }

    private void Look()
    {
        if (lookInput is { x: 0, y: 0 })
            return;
        
        //Get how much up/down and left/right the mouse moved
        var realSensitivity = logic.sensitivity / 500;
        var mouseX = lookInput.x * realSensitivity;
        var mouseY = lookInput.y * realSensitivity;
        
        //Rotate the camera up and down locally
        cameraXRotation -= mouseY;
        cameraXRotation = Mathf.Clamp(cameraXRotation, -90, 90);
        camera.transform.localRotation = Quaternion.Euler(cameraXRotation, 0, 0);
        
        //Rotate the player right and left
        playerYRotation += mouseX;
        transform.rotation = Quaternion.Euler(0, playerYRotation, 0);
    }

    private void SetRunAnimation()
    {
        //Show the running animation if the player is moving quick enough
        var speed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;
        var showRunAnimation = speed > logic.speedForRunAnimation && isOnGround;
        if (showRunAnimation != runAnimationIsOn)
        {
            animator.SetBool("IsRunning", showRunAnimation);
            runAnimationIsOn = showRunAnimation;
        }
    }

    private void Gravity()
    {
        if (!isOnGround)
            rb.AddForce(9.81f * logic.gravityScale * Vector3.down, ForceMode.Acceleration);
    }
    
    
    //------------
    //Player Input
    //------------
    
    public void OnMove(InputAction.CallbackContext context)
    {
        //moveInput is a vector in local space
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
            Debug.Log("Pew!");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpInput = true;
        else if (context.canceled)
            jumpInput = false;
    }
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (logic.toggleCrouch)
        {
            if (context.performed)
                shouldBeCrouched = !shouldBeCrouched;
        }
        else
        {
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
        rb.position = new Vector3(0, 0, -6);
        transform.position = new Vector3(0, 0, -6);
        camera.transform.rotation = Quaternion.identity;
    }
}
