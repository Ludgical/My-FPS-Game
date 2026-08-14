using UnityEngine;

namespace Generators
{
    public class Branch
    {
        public readonly float x;
        public readonly float y;
        public readonly Room parent;
        public readonly Vector2 direction;

        public Branch(float x, float y, Room parent, Vector2 direction)
        {
            this.x = x;
            this.y = y;
            this.parent = parent;
            this.direction = direction;
        }
    }
}