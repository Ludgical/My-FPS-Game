using UnityEngine;

public abstract class DronePathfindMethod
{
    public float smoothTime;
    /// -1 means no required max speed
    public float maxSpeed;
    protected Vector3 velocity;
    
    protected readonly Transform droneTransform;
    
    protected DronePathfindMethod(Transform droneTransform)
    {
        this.droneTransform = droneTransform;
    }
    
    public abstract void Pathfind();
}