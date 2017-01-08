using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entoarox.Framework.Abstraction
{
    public class Character
    {
        private StardewValley.NPC Char;
        internal Character(StardewValley.NPC character)
        {
            Char = character;
        }
        public string Name
        {
            get
            {
                return Char.name;
            }
        }
        internal GameLocation Location
        {
            get
            {
                return GameLocation.Wrap()
            }
        }
    }
}
