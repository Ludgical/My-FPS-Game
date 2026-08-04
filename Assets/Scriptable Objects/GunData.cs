using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/GunData")]
    public class GunData : ScriptableObject
    {
        [Tooltip("How far away from the origin should the gun move?")]
        public float positionOffsetMultiplier;
        [Tooltip("How much from the default rotation should the gun rotate?")]
        public float rotationOffsetMultiplier;
        [Tooltip("Smooth time when the gun goes to its goal offset, lower = faster")]
        public float positionOffsetSmoothTime;
        [Tooltip("How quickly should the rotation offset reach its goal offset?, higher = faster")]
        public float rotationOffsetSpeed;
        [Tooltip("Cooldown between shots")]
        public float cooldown;
        [Tooltip("How much damage every shot deals")]
        public float damage;
    }
}
