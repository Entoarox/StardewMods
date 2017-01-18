/* INDEV
using System;

using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Extensions
{
    public static class ConversionExtensions
    {
        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }
        public static Point ToPoint(this Vector2 vector, bool round = true)
        {
            if (!round && ((int)vector.X != vector.X || (int)vector.Y != vector.Y))
                throw new ArgumentException("Vector is not valid for conversion to point when rounding is disabled", nameof(vector));
            return new Point((int)vector.X, (int)vector.Y);
        }
    }
}
*/