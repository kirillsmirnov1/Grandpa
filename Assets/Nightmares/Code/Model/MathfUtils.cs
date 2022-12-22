using UnityEngine;

namespace Nightmares.Code.Model
{
    public static class MathfUtils
    {
        public static float Angle(Vector2 vector2)
        {
            return 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * Mathf.Sign(vector2.x));
        }
        
        /// <summary>
        /// Counts full circle counter clock wise from (1, 0)
        /// </summary>
        public static float ToAngleInDegrees(this Vector2 v)
        {
            var angle = Vector2.Angle(Vector2.right, v);
            if (v.y < 0) angle = 360 - angle;
            return angle;
        }
    }
}