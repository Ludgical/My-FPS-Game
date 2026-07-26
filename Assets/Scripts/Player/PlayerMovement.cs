using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private References refs;

    private PlayerScript player;
    [SerializeField] private PlayerInput input;
    [SerializeField] private CapsuleCollider collider;
    [SerializeField] public Rigidbody rb;
    
    private Coroutine crouchCoroutine;
    private float cameraYVelocity;
    
    private void Start()
    {
        refs = References.Refs;
        player = refs.player;
    }
    
    private void FixedUpdate()
    {
        //The player can't move if the game isn't on
        if (!refs.gameLogic.gameIsOn)
            return;
        
        //Crouch and stand up
        CrouchOrStandUp();
        //Move forward, backward, left, right and jump
        Move();
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

    private void CrouchOrStandUp()
    {
        //If the player's crouch state is already correct, return
        if (input.ShouldBeCrouched == player.IsCrouched)
            return;

        var colliderHeight = refs.playerData.colliderHeight;
        
        if (input.ShouldBeCrouched)
            Crouch();
        else
            StandUp();
        return;

        void Crouch()
        {
            player.SetIsCrouched(true);

            collider.height = colliderHeight / 2;
            collider.center = new Vector3(0, colliderHeight / 4, 0);
            
            if (crouchCoroutine != null)
                StopCoroutine(crouchCoroutine);
            crouchCoroutine = StartCoroutine(ChangeCameraHeight(refs.playerData.crouchHeight));
        }
        void StandUp()
        {
            player.SetIsCrouched(false);
            
            collider.height = colliderHeight;
            collider.center = new Vector3(0, colliderHeight / 2, 0);
            
            if (crouchCoroutine != null)
                StopCoroutine(crouchCoroutine);
            crouchCoroutine = StartCoroutine(ChangeCameraHeight(refs.playerData.standHeight));
        }
        
        IEnumerator ChangeCameraHeight(float targetHeight)
        {
            var camTransform = refs.camera.transform;
        
            //If the camera isn't very close to its goal position
            while (Mathf.Abs(targetHeight - camTransform.localPosition.y) > 0.01f)
            {
                //Move the camera closer to its goal position and wait for the next frame
                var camPosition = camTransform.localPosition;
                camPosition.y = Mathf.SmoothDamp(camPosition.y, targetHeight, ref cameraYVelocity, refs.playerData.crouchAnimationDampTime);
                camTransform.localPosition = camPosition;
                yield return null;
            }
        
            //Move the camera to its goal position
            var newPosition = camTransform.localPosition;
            newPosition.y = targetHeight;
            camTransform.localPosition = newPosition;
        }
    }

    private void Move()
    {
        //Get a movement vector from the direction the player is facing and the move input
        var movement = transform.forward * input.MoveInput.y + transform.right * input.MoveInput.x;
        
        //Change the max move speed based on if the player is crouched or not
        var maxMoveSpeed = player.IsCrouched ? refs.playerData.maxCrouchSpeed : refs.playerData.maxRunSpeed;
        //Change the acceleration speed based on if the player is in the air or not
        var movementLerpStep = player.IsOnGround ? refs.playerData.movementLerpStepGround : refs.playerData.movementLerpStepAir;
        
        //Use lerp functions to keep accelerating until a max speed
        var newXVelocity = Mathf.Lerp(
            rb.linearVelocity.x, movement.x * maxMoveSpeed, movementLerpStep);
        var newZVelocity = Mathf.Lerp(
            rb.linearVelocity.z, movement.z * maxMoveSpeed, movementLerpStep);
        
        //Set the velocities to 0 if they are very close to 0 (stop moving if the player is basically still)
        if (Mathf.Abs(newXVelocity) < 0.001f) newXVelocity = 0;
        if (Mathf.Abs(newZVelocity) < 0.001f) newZVelocity = 0;
        
        //Upwards velocity (jumping)
        var newYVelocity = input.JumpInput && player.IsOnGround ? refs.playerData.jumpStrength : rb.linearVelocity.y;

        //Apply the velocity to the rigidbody of the player
        rb.linearVelocity = new Vector3(newXVelocity, newYVelocity, newZVelocity);
    }

    private void Look()
    {
        //Return if the player didn't move the mouse
        if (input.LookInput is { x: 0, y: 0 })
            return;
        
        //Get how much up/down and left/right the mouse moved
        var realSensitivity = Settings.Player.Sensitivity.Value / 500;
        var mouseX = input.LookInput.x * realSensitivity;
        var mouseY = input.LookInput.y * realSensitivity;
        
        //Rotate the camera up and down locally
        var cameraXRotation = refs.camera.transform.localRotation.eulerAngles.x;
        if (cameraXRotation >= 270)
            cameraXRotation -= 360;
        cameraXRotation -= mouseY;
        cameraXRotation = Mathf.Clamp(cameraXRotation, -90, 90);
        refs.camera.transform.localRotation = Quaternion.Euler(cameraXRotation, 0, 0);
        
        //Rotate the player right and left
        var playerYRotation = transform.rotation.eulerAngles.y;
        playerYRotation += mouseX;
        transform.rotation = Quaternion.Euler(0, playerYRotation, 0);
    }
    
    private void Gravity()
    {
        //Apply gravity with a scale to the player
        if (!player.IsOnGround)
            rb.AddForce(9.81f * refs.playerData.gravityScale * Vector3.down, ForceMode.Acceleration);
    }
}
