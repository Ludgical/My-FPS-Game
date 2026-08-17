using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/TargetChallengeData")]
    public class TargetChallengeData : ScriptableObject
    {
        public float timeToGoToPlayerPos;
        public int depthToSpawnDrone;
        public float droneDistanceBehind;
        public float droneMinY;
        public float droneMaxY;
        public float droneSpeed;
        public float droneVelocitySmoothTime;
        public float droneSpeedDecreasePerRestart;
        public float droneLaserTime;
        public float destroyDroneTime;
        public float restartTime;
        public float nextLevelTime;
    }
}
