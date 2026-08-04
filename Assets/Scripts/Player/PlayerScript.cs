using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private References refs;
    
    [SerializeField] private PlayerInput input;
    [SerializeField] private CapsuleCollider collider;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public bool IsOnGround { get; private set; }
    public bool IsCrouched { get; private set; }
    
    private bool runAnimationIsOn;

    private void Start()
    {
        refs = References.Refs;
    }

    private void FixedUpdate()
    {
        //Check if the player is on the ground or not
        CheckIsOnGround();
        //Show the running animation if the player is running
        SetRunAnimation();
    }

    private void SetRunAnimation()
    {
        //The speed at which the player is moving
        var speed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;
        //The running animation can be enabled if the player is moving quick enough
        var showRunAnimation = speed > refs.playerData.speedForRunAnimation && IsOnGround;
        //Enable or disable the running animation if it's not already in the correct state
        if (showRunAnimation != runAnimationIsOn)
        {
            refs.playerAnimator.SetBool("IsRunning", showRunAnimation);
            runAnimationIsOn = showRunAnimation;
        }
    }
    
    private void CheckIsOnGround()
    {
        IsOnGround = Physics.CheckSphere(groundCheck.position, 1.18f, groundLayer, QueryTriggerInteraction.Ignore);
    }
    
    public void SetIsCrouched(bool crouched)
    {
        IsCrouched = crouched;
    }
}
