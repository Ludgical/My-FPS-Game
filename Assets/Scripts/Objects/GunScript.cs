using UnityEngine;

public class GunScript : MonoBehaviour
{
    private References refs;
    private Transform gunPivot;
    private Transform delayedFollowPivot;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gunFiringSound;
    [SerializeField] private MeshRenderer[] renderers;
    [SerializeField] private GameObject onHitParticles;

    private Vector3 gunVelocity;
    private Vector3 oldPivotPosition;
    private Quaternion oldPivotRotation;
    private Vector3 goalPositionOffset;
    private Quaternion goalRotationOffset;
    private float timeSinceLastShot;

    private void Start()
    {
        refs = References.Refs;
        
        gunPivot = refs.gunPivot;
        delayedFollowPivot = refs.delayedFollowPivot;
        renderers = GetComponentsInChildren<MeshRenderer>();
        
        SetVisible(false);
        SetZValue();
        
        refs.gameLogic.onPlay += () =>
        {
            SetVisible(true);
        };

        Settings.Player.FOV.onUpdated += SetZValue;
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }
    
    private void FixedUpdate()
    {
        //All the transforms update before FixedUpdate is called
        
        //How the gun moved between the last call to FixedUpdate and this one in global space
        var globalDirection = gunPivot.position - oldPivotPosition;
        //The same, but in local space of the delayedFollowPivot
        var localDirection = delayedFollowPivot.InverseTransformDirection(globalDirection);
        //Direction opposite of the one the gun moved in, and also a little shorter
        goalPositionOffset = localDirection * refs.gunData.positionOffsetMultiplier;

        //How the player rotated between the last call to FixedUpdate and this one
        var localRotation = gunPivot.rotation.eulerAngles - oldPivotRotation.eulerAngles;
        //Rotation opposite of the one the player rotated in
        goalRotationOffset = Quaternion.Euler(localRotation * refs.gunData.rotationOffsetMultiplier);
        
        //Set the oldPivotPosition and rotation for the next call to FixedUpdate
        oldPivotPosition = gunPivot.position;
        oldPivotRotation = gunPivot.rotation;
    }

    private void LateUpdate()
    {
        //Move the position offset towards its goal and give the movement a bit of lag
        delayedFollowPivot.localPosition = Vector3.SmoothDamp(
            delayedFollowPivot.localPosition, goalPositionOffset, 
            ref gunVelocity, refs.gunData.positionOffsetSmoothTime);
        
        //Move the rotation offset towards its goal and give the rotation a bit of lag
        delayedFollowPivot.localRotation = Quaternion.Lerp(
            delayedFollowPivot.localRotation, goalRotationOffset, 
            Time.deltaTime * refs.gunData.rotationOffsetSpeed);
    }

    private void SetZValue()
    {
        //Set the gun's z-value based on the player's fov
        var gunPos = refs.gunPivot.localPosition;
        gunPos.z = Settings.Player.FOV.Value / -150 + 1.24f;
        refs.gunPivot.localPosition = gunPos;
    }

    public void TryFire()
    {
        //Can't fire if enough time hasn't passed since the last shot
        if (timeSinceLastShot < refs.gunData.cooldown)
            return;
        timeSinceLastShot = 0;
        
        Fire();
    }

    private void Fire()
    {
        audioSource.PlayOneShot(gunFiringSound);
        refs.playerAnimator.SetTrigger("Fire");

        //Raycast forward from the camera and check if it hit something
        var start = refs.camera.transform.position;
        var direction = refs.camera.transform.forward;
        var raycast = Physics.Raycast(
            start, direction, out var hit, 
            500, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        if (raycast)
        {
            //If the raycast hit an IHittable, call the IHittable's OnHit method
            if (hit.collider.TryGetComponent<IHittable>(out var hittable))
                hittable.OnHit();
            
            OnHitParticles(hit.point, hit.normal);
        }
    }

    private void OnHitParticles(Vector3 position, Vector3 rotation)
    {
        //Create the particle object and destroy it after 0.5 seconds
        Destroy(Instantiate(onHitParticles, position, Quaternion.LookRotation(rotation)), 0.5f);
    }

    private void SetVisible(bool visible)
    {
        foreach (var renderer in renderers)
            renderer.enabled = visible;
    }
}
