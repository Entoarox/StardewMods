using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.Framework.Abstraction
{
    public class Farmer
    {
        private StardewValley.Farmer Who;
        internal Farmer(StardewValley.Farmer who)
        {
            Who = who;
        }
        public string Name => Who.name;
        public List<int> Achievements => Who.achievements;
        public int Accessory
        {
            get
            {
                return Who.accessory;
            }
            set
            {
                Who.accessory = value;
            }
        }
    }
}
