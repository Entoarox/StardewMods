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
            List<Pet> pets = this.GetAllPets();
            if (vanillaOnly)
                return pets.Any(a => (Game1.player.catPerson ? a is Cat : a is Dog) && a.Manners == 0 && a.Age == 0);
            else
                return pets.Any();
        }
        public Pet GetPet()
        {
            List<Pet> pets = new List<Pet>();
            List<NPC> npcs = new List<NPC>();
            foreach (NPC npc in Utility.getAllCharacters())
            {
                npcs.Add(npc);
            }
            pets = npcs.Where(a => a is Pet).Cast<Pet>().ToList();
            Pet pet = pets.First(a => (Game1.player.catPerson ? a is Cat : a is Dog) && a.Manners == 0 && a.Age == 0);
            if (pet == null && pets.Any())
                pet = pets.First();
            return pet;
        }
        public List<Pet> GetAllPets()
        {
            List<Pet> pets = new List<Pet>();
            List<NPC> npcs = new List<NPC>();
            foreach (NPC npc in Utility.getAllCharacters())
            {
                npcs.Add(npc);
            }
            pets = npcs.Where(a => a is Pet).Cast<Pet>().ToList();
            return pets;
        }
    }
}
