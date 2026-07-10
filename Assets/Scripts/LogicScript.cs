using UnityEngine;

public class LogicScript : MonoBehaviour
{
    [Header("Constants")]
    public float maxRunSpeed;
    public float maxCrouchSpeed;
    public float jumpStrength;
    public float gravityScale;
    public float colliderHeight;
    public float movementLerpStepGround;
    public float movementLerpStepAir;
    public float speedForRunAnimation;
    
    [Header("Settings")]
    public float sensitivity;
    public float fov;
    public bool toggleCrouch;
    
    private void Start()
    {
        //Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
