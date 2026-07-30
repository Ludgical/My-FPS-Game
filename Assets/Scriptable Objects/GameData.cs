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
    }
}
