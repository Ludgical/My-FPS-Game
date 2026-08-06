using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/DroneData")]
    public class DroneData : ScriptableObject
    {
        public GameObject dronePrefab;
        public float droneSpawnMinY;
        public float droneSpawnMaxY;
        public float laserWidth;
    }
}
