using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/GameData")]
    public class GameData : ScriptableObject
    {
        public float completedToResetDelay;
        public float fadeToBlackDuration;
        public float fadeFromBlackDuration;
        public float wallY;
        
        /// Distance from the center of a challenge room to a wall of the challenge room
        // [Tooltip("Distance from the center of a challenge room to a wall of the challenge room")]
        public float CenterToWall;
    
        /// Distance from the center of a challenge room to a door of the challenge room
        public float CenterToDoor => CenterToWall + 1;
    
        /// Distance from the center of a challenge room to the center of an adjacent challenge room
        public float CenterToCenter => CenterToDoor * 2;
    }
}
