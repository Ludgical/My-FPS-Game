using System.Collections.Generic;
using UnityEngine;

namespace Generators
{
    public class Room
    {
        public readonly float x;
        public readonly float y;
        public readonly int depth;
        public readonly Vector2 entranceSide;
        // Side of the room : prefab
        public readonly Dictionary<Vector2, GameObject> walls = new();
        public readonly Dictionary<Vector2, GameObject> doors = new();

        public Room(float x, float y, int depth, Vector2 entranceSide)
        {
            this.x = x;
            this.y = y;
            this.depth = depth;
            this.entranceSide = entranceSide;
            foreach (var direction in MapGenerator.Directions)
            {
                walls[direction] = null;
                doors[direction] = null;
            }
        }
    }
}
