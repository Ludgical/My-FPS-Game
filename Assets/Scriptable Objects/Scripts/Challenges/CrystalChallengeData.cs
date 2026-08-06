using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/CrystalChallengeData")]
    public class CrystalChallengeData : ScriptableObject
    {
        [Tooltip("How many degrees the crystal should rotate every second")]
        public float crystalRotationPerSecond = 60;
        [Tooltip("How long it takes the crystal to bob up and down and go back to its start position")]
        public float crystalBobDurationSeconds = 2.5f;
        [Tooltip("How much the crystal should move up and down")]
        public float crystalBobHeightMultiplier = 0.2f;
        public int crystalAmount;
        [Tooltip("How far from the center crystals can spawn")]
        public float outerRadius;
        [Tooltip("How close to the center crystals can spawn")]
        public float innerRadius;
        public float crystalSpawnMinY;
        public float crystalSpawnMaxY;
        public float droneHealth;
        [Tooltip("Minimum amount of time between drones spawning")]
        public float droneSpawnMinDelay;
        [Tooltip("Maximum amount of time between drones spawning")]
        public float droneSpawnMaxDelay;
        [Tooltip("How quickly the drone should move")]
        public float droneSmoothTime;
        public float droneMaxSpeed;
        public float droneRotationSpeed;
        public float droneLaserActiveTime;
        [Tooltip("How long the challenge should pause after the player gets hit by a drone")]
        public float waitingTime;
    }
}
