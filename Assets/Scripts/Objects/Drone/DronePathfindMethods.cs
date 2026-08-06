using UnityEngine;

public static class DronePathfindMethods
{
    public class TowardsPlayer : DronePathfindMethod
    {
        public float rotationSpeed;
        
        public TowardsPlayer(Drone drone) : base(drone) { }

        protected override void Pathfind()
        {
            var playerPosition = Challenge.GetPlayerPosition();

            //Use SmoothDamp to move the drone towards the player
            droneTransform.position = maxSpeed != -1f
                ? Vector3.SmoothDamp(
                    droneTransform.position, playerPosition, ref velocity, smoothTime, maxSpeed)
                : Vector3.SmoothDamp(
                    droneTransform.position, playerPosition, ref velocity, smoothTime);
            
            //Use Slerp to make the drone face towards the player
            var playerDirection = playerPosition - droneTransform.position;
            var targetRotation = Quaternion.LookRotation(playerDirection);
            droneTransform.rotation = Quaternion.Slerp(
                droneTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    public class Circle : DronePathfindMethod
    {
        public Vector3 center;
        public float radius;
        public bool faceCenter;
        
        public Circle(Drone drone) : base(drone) { }
        
        protected override void Pathfind()
        {
            
        }
    }

    public class GoAndStay : DronePathfindMethod
    {
        public GoAndStay(Drone drone) : base(drone) { }
        
        public Vector3 position;
        public Vector3 facing;
        
        protected override void Pathfind()
        {
            
        }
    }
}
