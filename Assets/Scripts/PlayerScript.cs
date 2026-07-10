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
    private float cameraYRotation;
    
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
        //Crouch and stand up
        Crouch();
        //Move forward, backward, left, right and jump
        Move();
        //Show the running animation if the player is running
        SetRunAnimation();
        //Gravity with scale
        Gravity();
        
        //Move to when the fov slider changes (temporary)
        SetFOV(logic.fov);
    }
    
    private void Update()
    {
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
        //moveInput is a Vector2, so I use .x and .y
        //movement is a Vector3, so I use .x and .z
        //movement is a vector in world space
        var movement = camera.transform.forward * moveInput.y + camera.transform.right * moveInput.x;
        
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
        
        //Rotate the camera locally
        //up/down
        cameraXRotation -= mouseY;
        cameraXRotation = Mathf.Clamp(cameraXRotation, -90, 90);
        
        //left/right
        cameraYRotation += mouseX;
        
        camera.transform.localRotation = Quaternion.Euler(cameraXRotation, cameraYRotation, 0);
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
            //Temporary
            door.OpenDoor();
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
}
