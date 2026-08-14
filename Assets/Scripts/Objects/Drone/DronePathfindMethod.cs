using UnityEngine;

public abstract class DronePathfindMethod
{
    protected readonly Drone drone;
    protected readonly Transform droneTransform;
    
    public float velocitySmoothTime;
    public float maxSpeed;
    protected Vector3 velocity;
    
    private bool isFrozen;
    private bool initialized;
    
    protected DronePathfindMethod(Drone drone)
    {
        this.drone = drone;
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
        if (!initialized)
        {
            Initialize();
            initialized = true;
        }

        if (!isFrozen)
            Pathfind();
    }
    
    protected abstract void Initialize();
    
    protected abstract void Pathfind();

    protected void RotateTowards(Vector3 target, float rotationSpeed = 5)
    {
        var direction = target - droneTransform.position;
        if (direction == Vector3.zero)
            return;
        
        droneTransform.rotation = Quaternion.Slerp(
            droneTransform.rotation,
            Quaternion.LookRotation(target - droneTransform.position),
            rotationSpeed * Time.deltaTime);
    }
}