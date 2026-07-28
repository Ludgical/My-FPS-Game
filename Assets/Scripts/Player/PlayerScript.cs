using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private References refs;
    
    [SerializeField] private PlayerInput input;
    [SerializeField] private CapsuleCollider collider;
    [SerializeField] private Rigidbody rb;

    public bool IsOnGround { get; private set; }
    public bool IsCrouched { get; private set; }
    
    private bool runAnimationIsOn;

    private void Start()
    {
        refs = References.Refs;
    }

    private void FixedUpdate()
    {
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
    
    public void SetIsOnGround(bool onGround)
    {
        IsOnGround = onGround;
    }
    public void SetIsCrouched(bool crouched)
    {
        IsCrouched = crouched;
    }
}
