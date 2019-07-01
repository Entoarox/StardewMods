using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.Locations;

namespace MagicJunimoPet
{
    public abstract class Behaviour
    {
        protected readonly SmartPet Pet;
        public Behaviour(SmartPet pet)
        {
            this.Pet = pet;
        }
        public abstract void OnBehaviour(GameTime time);
    }
}
