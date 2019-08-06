using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Newtonsoft.Json;

namespace SundropCity.Json
{
    class CarType
    {
        public string Base;
        public string Recolor;
        public Dictionary<string, Color> Source = new Dictionary<string, Color>();
        public Dictionary<string, string> Decals = new Dictionary<string, string>();
        public RecolorType[] Variants = new RecolorType[0];
    }
}
