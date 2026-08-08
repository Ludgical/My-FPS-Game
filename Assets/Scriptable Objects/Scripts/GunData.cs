using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/GunData")]
    public class GunData : ScriptableObject
    {
        [Header("0 - Gun Sway: None, 1 - Gun Sway: Low, 2 - Gun Sway: High")]
        [Tooltip("How far away from the origin should the gun move?")]
        public float[] positionOffsetMultipliers;
        [Tooltip("How much from the default rotation should the gun rotate?")]
        public float[] rotationOffsetMultipliers;
        [Tooltip("Smooth time when the gun goes to its goal offset, lower = faster")]
        public float[] positionOffsetSmoothTimes;
        [Tooltip("How quickly should the rotation offset reach its goal offset?, higher = faster")]
        public float[] rotationOffsetSpeeds;
        [Tooltip("Cooldown between shots")]
        public float cooldown;
        [Tooltip("How much damage every shot deals")]
        public float damage;
    }
}
