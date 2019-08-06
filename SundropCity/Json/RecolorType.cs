using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace SundropCity.Json
{
    class RecolorType
    {
        public Dictionary<string, Color> Palette = new Dictionary<string, Color>();
        public string[] Disallow = new string[0];
    }
}
