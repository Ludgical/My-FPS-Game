using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private References refs;
    
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool ShouldBeCrouched { get; private set; }

    private void Start()
    {
        refs = References.Refs;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //Vector showing the player's WASD input
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //Vector showing the player's mouse movement
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        //Fire when left click is pressed
        if (context.performed && refs.gameLogic.gameIsOn)
            refs.gun.TryFire();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //When space is held down, jumpInput = true
        if (context.performed)
            JumpInput = true;
        else if (context.canceled)
            JumpInput = false;
    }
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (Settings.Player.ToggleCrouch)
        {
            //If toggle crouch, change state when the crouch button is pressed
            if (context.performed)
                ShouldBeCrouched = !ShouldBeCrouched;
        }
        else
        {
            //If not toggle crouch, crouch when pressed and uncrouch when let go
            if (context.performed)
                ShouldBeCrouched = true;
            else if (context.canceled)
                ShouldBeCrouched = false;
        }
    }
}
