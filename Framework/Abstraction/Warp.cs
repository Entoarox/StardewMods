using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.Framework.Abstraction
{
    public class Warp
    {
        public readonly string Destination;
        public readonly int X;
        public readonly int Y;
        public Warp(string destination, int x, int y)
        {
            Destination = destination;
            X = x;
            Y = y;
        }
    }
}
