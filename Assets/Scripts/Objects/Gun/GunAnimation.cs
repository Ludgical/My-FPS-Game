using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    private References refs;
    
    private Transform gunPivot;
    private Transform delayedFollowPivot;
    
    private Vector3 gunVelocity;
    private Vector3 oldPivotPosition;
    private Quaternion oldPivotRotation;
    private Vector3 goalPositionOffset;
    private Quaternion goalRotationOffset;

    private int gunSwayLevel;
    private bool runAnimationIsOn;
    
    private void Start()
    {
        refs = References.Refs;
        
        gunPivot = refs.gunPivot;
        delayedFollowPivot = refs.delayedFollowPivot;
        
        SetGunSwayLevel();
        Settings.Player.GunSway.onUpdated += SetGunSwayLevel;
    }
    
    private void FixedUpdate()
    {
        SetGoalPositionOffset();
        SetGoalRotationOffset();
    }

    private void SetGoalPositionOffset()
    {
        //How the gun moved between the last call to FixedUpdate and this one in global space
        var globalDirection = gunPivot.position - oldPivotPosition;
        //The same, but in local space of the delayedFollowPivot
        var localDirection = delayedFollowPivot.InverseTransformDirection(globalDirection);
        //Direction opposite of the one the gun moved in, multiplied by the position offset multiplier
        var multiplier = refs.gunData.positionOffsetMultipliers[gunSwayLevel];
        goalPositionOffset = -localDirection * multiplier;
        
        //Set the oldPivotPosition for the next call to FixedUpdate
        oldPivotPosition = gunPivot.position;
    }

    private void SetGoalRotationOffset()
    {
        //How the player rotated between the last call to FixedUpdate and this one
        var rotation = gunPivot.rotation.eulerAngles - oldPivotRotation.eulerAngles;
        rotation.x = Mathf.DeltaAngle(0, rotation.x);
        rotation.y = Mathf.DeltaAngle(0, rotation.y);
        
        //Rotation opposite of the one the player rotated in, multiplied by the rotation offset multiplier
        var multiplier = refs.gunData.rotationOffsetMultipliers[gunSwayLevel];
        var eulerOffset = -rotation * multiplier;
        goalRotationOffset = Quaternion.Euler(eulerOffset);
        
        //Set the oldPivotRotation for the next call to FixedUpdate
        oldPivotRotation = gunPivot.rotation;
    }

    private void LateUpdate()
    {
        //Move the position offset towards its goal and give the movement a bit of lag
        delayedFollowPivot.localPosition = Vector3.SmoothDamp(
            delayedFollowPivot.localPosition, goalPositionOffset, 
            ref gunVelocity, refs.gunData.positionOffsetSmoothTimes[gunSwayLevel]);
        
        //Move the rotation offset towards its goal and give the rotation a bit of lag
        delayedFollowPivot.localRotation = Quaternion.Lerp(
            delayedFollowPivot.localRotation, goalRotationOffset, 
            Time.deltaTime * refs.gunData.rotationOffsetSpeeds[gunSwayLevel]);
    }

    public void SetRunAnimation(Vector3 velocity)
    {
        //The speed at which the player is moving
        var speed = new Vector2(velocity.x, velocity.z).magnitude;
        //The running animation can be enabled if the player
        //is moving quick enough and is on the ground
        var quickEnough = speed > refs.playerData.speedForRunAnimation;
        var showRunAnimation = quickEnough && refs.player.IsOnGround;
        //Enable or disable the running animation if it's not already in the correct state
        if (showRunAnimation != runAnimationIsOn)
        {
            refs.playerAnimator.SetBool("IsRunning", showRunAnimation);
            runAnimationIsOn = showRunAnimation;
        }
    }

    private void SetGunSwayLevel()
    {
        gunSwayLevel = Settings.Player.GunSway.ValueIndex;
        refs.playerAnimator.SetInteger("GunSwayLevel", gunSwayLevel);
    }
}
