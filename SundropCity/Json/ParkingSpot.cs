using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SundropCity.Json
{
    class ParkingSpot
    {
        public Facing[] Facings;
        public Vector2 Target;
        public ParkingSpot(Vector2 target, Facing[] facings)
        {
            this.Target = target;
            this.Facings = facings;
        }
    }
}
