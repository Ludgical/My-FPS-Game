using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public float maxRunSpeed;
        public float maxCrouchSpeed;
        public float jumpStrength;
        public float gravityScale;
        public float colliderHeight;
        public float crouchedColliderHeight => colliderHeight / 2;
        public float movementLerpStepGround;
        public float movementLerpStepAir;
        public float speedForRunAnimation;
        public float standHeight;
        public float crouchHeight;
        public float crouchAnimationDampTime;
    }
}
