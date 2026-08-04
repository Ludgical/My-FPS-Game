using UnityEngine;

public static class DronePathfindMethods
{
    public class TowardsPlayer : DronePathfindMethod
    {
        public TowardsPlayer(Transform droneTransform) : base(droneTransform) { }

        public override void Pathfind()
        {
            var playerPosition = References.Refs.player.transform.position;

            droneTransform.position = maxSpeed != -1
                ? Vector3.SmoothDamp(
                    droneTransform.position, playerPosition, ref velocity, smoothTime, maxSpeed)
                : Vector3.SmoothDamp(
                    droneTransform.position, playerPosition, ref velocity, smoothTime);
            
            droneTransform.LookAt(playerPosition);
        }
    }
    
    public class Circle : DronePathfindMethod
    {
        public Vector3 center;
        public float radius;
        public bool faceCenter;
        
        public Circle(Transform droneTransform) : base(droneTransform) { }
        
        public override void Pathfind()
        {
            
        }
    }

    public class GoAndStay : DronePathfindMethod
    {
        public GoAndStay(Transform droneTransform) : base(droneTransform) { }
        
        public Vector3 position;
        public Vector3 facing;
        
        public override void Pathfind()
        {
            
        }
    }
}
