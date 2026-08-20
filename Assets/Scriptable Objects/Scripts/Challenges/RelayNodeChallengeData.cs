using UnityEngine;

namespace Scriptable_Objects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/RelayNodeChallengeData")]
    public class RelayNodeChallengeData : ScriptableObject
    {
        public int nodeAmount;
        public float radius;
        public float minY;
        public float maxY;
        public float nodeMinDistance;
        public float waitTime;
        public float maxTimeBetweenHits;
        public float timeBetweenHitsIncrease;
        public float fadeOutSpeed;
        public float fadeScreenImageTime;
    }
}
