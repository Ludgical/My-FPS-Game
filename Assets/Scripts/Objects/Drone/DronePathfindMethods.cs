using UnityEngine;

public static class DronePathfindMethods
{
    public class TowardsPlayer : DronePathfindMethod
    {
        public float rotationSpeed;
        public float speedIncreasePerSecond;

        private float time;
        private Vector3 velocityVelocity;
        
        public TowardsPlayer(Drone drone) : base(drone) { }
        
        protected override void Initialize() { }

        protected override void Pathfind()
        {
            var playerPosition = Challenge.GetPlayerPosition();

            var targetSpeed = maxSpeed + speedIncreasePerSecond * time;
            var targetVelocity = (playerPosition - droneTransform.position).normalized * targetSpeed;
            velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref velocityVelocity, velocitySmoothTime);
            
            droneTransform.position += velocity * Time.deltaTime;
            
            RotateTowards(playerPosition, rotationSpeed);
            
            time += Time.deltaTime;
        }
    }
    
    
    public class Circle : DronePathfindMethod
    {
        public Vector3 center;
        public float radius;
        public bool faceCenter;
        public bool smoothOnTransition;
        public int rotationDirection;

        private GoAndStay goAndStay;
        private Vector3 startPos;
        private Vector3 circleStart;
        private Quaternion circleRotation;
        private float lapTime;
        private float time;
        private bool goingToCircle = true;
        private bool smoothing;
        private Vector3 smoothVelocity;

        public Circle(Drone drone) : base(drone) { }

        protected override void Initialize()
        {
            //Calculate how the circle the drone moves around is rotated along the y-axis
            startPos = droneTransform.position;
            var direction = center - startPos;
            direction.y = 0;
            circleRotation = Quaternion.LookRotation(direction);
            
            //Calculate where the drone should start moving around the circle
            var angle = 90 * rotationDirection;
            var circleStartDirection = Quaternion.AngleAxis(angle, Vector3.up) * direction;
            circleStart = center + circleStartDirection.normalized * radius;
            
            Vector3 goAndStayPosition;
            if (smoothOnTransition)
            {
                var offset = (startPos - circleStart).normalized * (maxSpeed * 0.3f);
                offset += (circleStart - center).normalized * (radius * 0.05f);
                offset.y /= 4;
                goAndStayPosition = circleStart + offset;
            }
            else
                goAndStayPosition = circleStart;

            smoothVelocity = (goAndStayPosition - startPos).normalized * maxSpeed;
            
            goAndStay = new GoAndStay(drone)
            {
                velocitySmoothTime = velocitySmoothTime,
                maxSpeed = maxSpeed,
                targetPos = goAndStayPosition,
                facing = goAndStayPosition
            };
            
            lapTime = (2 * Mathf.PI * radius) / maxSpeed;
        }
        
        protected override void Pathfind()
        {
            if (goingToCircle)
            {
                GoToCircle();
                return;
            }

            if (smoothing)
            {
                SmoothTransition();
                return;
            }
            
            MoveAroundCircle();

            time += Time.deltaTime;
            time %= lapTime;
        }

        private void GoToCircle()
        {
            goAndStay.TryPathfind();
            if (goAndStay.isAtTargetPosition)
            {
                goingToCircle = false;
                if (smoothOnTransition)
                    smoothing = true;
            }
        }

        private void SmoothTransition()
        {
            var newPos = Vector3.SmoothDamp(
                droneTransform.position, circleStart, 
                ref smoothVelocity, 0.2f);
            var movement = (newPos - droneTransform.position).normalized * (maxSpeed * Time.deltaTime);

            var distFromStart = Vector3.Distance(droneTransform.position, circleStart);
            if (distFromStart <= Mathf.Max(0.03f, movement.magnitude))
            {
                droneTransform.position = circleStart;
                smoothing = false;
            }
            else
                droneTransform.position += movement;
            
            RotateTowards(circleStart);
        }

        private void MoveAroundCircle()
        {
            var angle = (time / lapTime) * 2 * Mathf.PI;
            var x = Mathf.Cos(angle) * radius * rotationDirection;
            var z = Mathf.Sin(angle) * radius;
            var offset = circleRotation * new Vector3(x, 0, z);
            var newPos = center + offset;

            RotateTowards(faceCenter ? center : newPos, 10);

            droneTransform.position = newPos;
        }
    }
    

    public class GoAndStay : DronePathfindMethod
    {
        public Vector3 targetPos;
        public Vector3 facing;

        public bool isAtTargetPosition;
        private Vector3 velocityVelocity;
        
        public GoAndStay(Drone drone) : base(drone) { }
        
        protected override void Initialize() { }
        
        protected override void Pathfind()
        {
            if (isAtTargetPosition)
                return;
            
            var dronePos = droneTransform.position;
            var targetVelocity = (targetPos - dronePos).normalized * maxSpeed;
            velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref velocityVelocity, velocitySmoothTime);

            var movement = velocity * Time.deltaTime;
            if (Vector3.Distance(dronePos, targetPos) <= movement.magnitude)
            {
                droneTransform.position = targetPos;
                isAtTargetPosition = true;
            }
            else
                droneTransform.position += movement;
            
            RotateTowards(facing);
        }
    }
}
