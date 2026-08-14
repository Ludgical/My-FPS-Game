using UnityEngine;

namespace Generators
{
    public class Rectangle
    {
        //Sides of the rectangle
        private readonly float x1;
        private readonly float x2;
        private readonly float y1;
        private readonly float y2;
    
        /// Create a rectangle where <c>x1</c> and <c>x2</c> are the x-coordinates of the vertical sides and
        /// <c>y1</c> and <c>y2</c> are the y-coordinates of the horizontal sides
        public Rectangle(float x1, float x2, float y1, float y2)
        {
            this.x1 = Mathf.Min(x1, x2);
            this.x2 = Mathf.Max(x1, x2);
            this.y1 = Mathf.Min(y1, y2);
            this.y2 = Mathf.Max(y1, y2);
        }

        /// Create a square centered at <c>x</c>, <c>y</c> that is the size of a challenge room
        public Rectangle(float x, float y) : this(
            x - CenterToDoor, x + CenterToDoor,
            y - CenterToDoor, y + CenterToDoor)
        { }

        private static float CenterToDoor => References.Refs.gameData.CenterToDoor;

        public bool CollidesWith(Rectangle rect)
        {
            return rect.x1 < x2 && rect.x2 > x1 && rect.y1 < y2 && rect.y2 > y1;
        }
    }
}
