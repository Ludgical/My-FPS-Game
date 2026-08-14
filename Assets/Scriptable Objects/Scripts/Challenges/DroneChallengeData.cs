using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/DroneChallengeData")]
    public class DroneChallengeData : ScriptableObject
    {
        public int dronesToDestroy;
        public int immediateDroneSpawnCount;
        public float droneSpawnDelay;
        public int maxDroneAmount;
        public float droneHealth;
        public float droneVelocitySmoothTime;
        public float droneMaxSpeed;
        public float dronePathfindMinRadius;
        public float dronePathfindMaxRadius;
        public float dronePathfindMinY;
        public float dronePathfindMaxY;
        public float chasingDroneVelocitySmoothTime;
        public float chasingDroneMaxSpeed;
        public float chasingDroneRotationSpeed;
        public float chasingDroneFreezeTime;
        public float chasingDroneY;
    }
}
