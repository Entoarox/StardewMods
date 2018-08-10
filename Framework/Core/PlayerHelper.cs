using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using StardewValley;
using StardewValley.Characters;

namespace Entoarox.Framework.Core
{
    internal class PlayerHelper : IPlayerHelper
    {
        private IPlayerModifierHelper _Modifiers;
        public IPlayerModifierHelper Modifiers
        {
            get
            {
                if (this._Modifiers == null)
                    this._Modifiers = new PlayerModifierHelper();
                return this._Modifiers;
            }
        }
        public void MoveTo(int x, int y)
        {
            Game1.warpFarmer(Game1.player.currentLocation.Name, Convert.ToInt32(x), Convert.ToInt32(y), Game1.player.facingDirection, Game1.player.currentLocation.isStructure.Value);
        }
        public void MoveTo(string location, int x, int y)
        {
            GameLocation loc = Game1.getLocationFromName(location);
            if (loc == null)
                throw new ArgumentNullException(nameof(location));
            MoveTo(loc, x, y);
        }
        public void MoveTo(GameLocation location, int x, int y)
        {
            Game1.warpFarmer(location.Name, Convert.ToInt32(x), Convert.ToInt32(y), Game1.player.facingDirection, location.isStructure.Value);
        }
        public bool HasPet(bool vanillaOnly)
        {
            List<NPC> matches = null;
            foreach(NPC n in Utility.getAllCharacters())
            {
                if(n is Pet)
                {
                    matches.Add(n);
                }
            }
            
            if (vanillaOnly)
                return matches.Any(a => (Game1.player.catPerson ? a is Cat : a is Dog) && a.Manners == 0 && a.Age == 0);
            else
                return matches.Any();
        }
        public Pet GetPet()
        {
            List<Pet> matches = null;
            foreach(NPC n in Utility.getAllCharacters())
            {
                if(n is Pet)
                {
                    matches.Add(n);
                }
            }
            Pet pet = matches.First(a => (Game1.player.catPerson ? a is Cat : a is Dog) && a.Manners == 0 && a.Age == 0);
            if (pet == null && matches.Any())
                pet = matches.First();
            return pet;
        }
        public Pet[] GetAllPets()
        {
            Pet[] pets = null;
            foreach (NPC n in Utility.getAllCharacters())
            {
                pets.Add(n);
            }
            return pets;
        }
    }
}
