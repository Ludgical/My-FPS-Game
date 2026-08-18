using System;
using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private References refs;
    
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GunAnimation gunAnimation;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] public Animator animator;

    private Coroutine footstepAudioRoutine;

    public bool IsOnGround { get; private set; }
    public bool IsCrouched { get; private set; }
    public bool IsRunning { get; private set; }

    [NonSerialized] public bool canCrouch = true;

    private void Start()
    {
        refs = References.Refs;
    }

    private void FixedUpdate()
    {
        CheckIsOnGround();
        CheckIsRunning();
        
        //Show the running animation if the player is running
        gunAnimation.SetRunAnimation();

        CheckPlayFootstepAudio();
    }
    
    private void CheckIsOnGround()
    {
        IsOnGround = Physics.CheckSphere(
            groundCheck.position, refs.playerData.colliderRadius - 0.02f,
            groundLayer, QueryTriggerInteraction.Ignore);
    }

    private void CheckIsRunning()
    {
        if (!IsOnGround || IsCrouched)
        {
            IsRunning = false;
            return;
        }
        
        var speed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;
        IsRunning = speed > refs.playerData.speedForRunning;
    }

    private void CheckPlayFootstepAudio()
    {
        //If the player is running, but the sounds aren't playing
        if (IsRunning && footstepAudioRoutine == null)
        {
            footstepAudioRoutine = StartCoroutine(FootstepAudioRoutine());
        }
        
        //If the player isn't running, but the sounds are playing
        else if (!IsRunning && footstepAudioRoutine != null)
        {
            StopCoroutine(footstepAudioRoutine);
            footstepAudioRoutine = null;
        }
    }
    
    public void SetIsCrouched(bool crouched)
    {
        IsCrouched = crouched;
    }

    public void FreezeMovement()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        canCrouch = false;
    }

    public void UnfreezeMovement()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        canCrouch = true;
    }

    private IEnumerator FootstepAudioRoutine()
    {
        while (true)
        {
            audioSource.PlayOneShot(footstepSound);
            yield return new WaitForSeconds(refs.playerData.footstepSoundDelay);
        }
    }
}
