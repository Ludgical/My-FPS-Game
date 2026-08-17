using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GunAnimation gunAnimation;
    [SerializeField] public Animator animator;

    public bool IsOnGround { get; private set; }
    public bool IsCrouched { get; private set; }

    private void FixedUpdate()
    {
        //Check if the player is on the ground or not
        CheckIsOnGround();
        //Show the running animation if the player is running
        gunAnimation.SetRunAnimation(rb.linearVelocity);
    }
    
    private void CheckIsOnGround()
    {
        IsOnGround = Physics.CheckSphere(groundCheck.position, 1.18f, groundLayer, QueryTriggerInteraction.Ignore);
    }
    
    public void SetIsCrouched(bool crouched)
    {
        IsCrouched = crouched;
    }

    public void FreezeMovement()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void UnfreezeMovement()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
