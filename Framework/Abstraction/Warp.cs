using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.Framework.Abstraction
{
    public class Warp
    {
        public readonly int X;
        public readonly int Y;
        public readonly string Target;
        public readonly int TargetX;
        public readonly int TargetY;
        public Warp(int x, int y, string target, int targetX, int targetY)
        {
            X = x;
            Y = y;
            Target = target;
            TargetX = targetX;
            TargetY = targetY;
        }
        internal static Warp Wrap(StardewValley.Warp obj)
        {
            return new Warp(obj.X, obj.Y, obj.TargetName, obj.TargetX, obj.TargetY);
        }
        internal static StardewValley.Warp Unwrap(Warp obj)
        {
            return new StardewValley.Warp(obj.X, obj.Y, obj.Target, obj.TargetX, obj.TargetY, false);
        }
    }
}
