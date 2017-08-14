using System;
using System.Linq;

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
                if (_Modifiers == null)
                    _Modifiers = new PlayerModifierHelper();
                return _Modifiers;
            }
        }
        public void MoveTo(int x, int y)
        {
            Game1.warpFarmer(Game1.player.currentLocation, Convert.ToInt32(x), Convert.ToInt32(y), Game1.player.facingDirection, Game1.player.currentLocation.isStructure);
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
            Game1.warpFarmer(location, Convert.ToInt32(x), Convert.ToInt32(y), Game1.player.facingDirection, location.isStructure);
        }
        public bool HasPet(bool vanillaOnly)
        {
            var matches = Utility.getAllCharacters().Where(a => a is Pet);
            if (vanillaOnly)
                return matches.Any(a => (Game1.player.catPerson ? a is Cat : a is Dog) && a.manners == 0 && a.age == 0);
            else
                return matches.Any();
        }
        public Pet GetPet()
        {
            var matches = Utility.getAllCharacters().Where(a => a is Pet).Cast<Pet>();
            Pet pet = matches.First(a => (Game1.player.catPerson ? a is Cat : a is Dog) && a.manners == 0 && a.age == 0);
            if (pet == null && matches.Any())
                pet = matches.First();
            return pet;
        }
        public Pet[] GetAllPets()
        {
            return Utility.getAllCharacters().Where(a => a is Pet).Cast<Pet>().ToArray();
        }
    }
}
