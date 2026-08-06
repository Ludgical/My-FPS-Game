using UnityEngine;

public abstract class DronePathfindMethod
{
    protected readonly Transform droneTransform;
    
    public float smoothTime;
    /// -1 means no required max speed
    public float maxSpeed;
    protected Vector3 velocity;
    private bool isFrozen;
    
    protected DronePathfindMethod(Drone drone)
    {
        droneTransform = drone.GetComponentInParent<Transform>();
    }

    public void Freeze()
    {
        velocity = Vector3.zero;
        isFrozen = true;
    }
    public void Unfreeze()
    {
        isFrozen = false;
    }

    public void TryPathfind()
    {
        if (!isFrozen)
            Pathfind();
    }
    
    protected abstract void Pathfind();
}