using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SundropCity.Hotel
{
    abstract class Furniture
    {
#pragma warning disable CS0649
        public string Type;
        public int Subtype;
        public int Picture;
        public bool IsLit;
        public Furniture[] Slotted;
#pragma warning disable CS0649

        public abstract void Draw(SpriteBatch b);
    }
}
