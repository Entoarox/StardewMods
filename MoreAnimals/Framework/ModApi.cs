using System;
using System.Collections.Generic;

using StardewModdingAPI;

using StardewValley.Characters;

namespace Entoarox.MorePetsAndAnimals.Framework
{
    public class ModApi
    {
        /// <summary>
        /// Registers a Pet, for which new versions can be found.
        /// </summary>
        /// <param name="id">The filename for the pet, which doubles as its id</param>
        /// <param name="type">The Type object which needs to have a instance created for this pet (Must inherit from Pet)</param>
        public void RegisterPetType(string id, Type type)
        {
            if (!typeof(Pet).IsAssignableFrom(type))
                ModEntry.SMonitor.Log("Unable to register pet type, type does not inherit from Pet class: " + id, LogLevel.Error);
            id = ModEntry.Sanitize(id);
            if(ModEntry.Pets.ContainsKey(id))
            {
                ModEntry.SMonitor.Log("Unable to register pet type, type already registered: " + id, LogLevel.Error);
                return;
            }
            ModEntry.Pets.Add(id, new List<AnimalSkin>());
            ModEntry.PetTypes.Add(id, type);
            ModEntry.PetTypesRev.Add(type, id);
        }
        /// <summary>
        /// Registers a new animal type to handle skin support for.
        /// </summary>
        /// <param name="id">The filename for the animal, which doubles as its id</param>
        /// <param name="hasBaby">If this animal type has a baby skin</param>
        /// <param name="canShear">If this animal type has a sheared skin</param>
        public void RegisterAnimalType(string id, bool hasBaby=true, bool canShear=false)
        {
            id = ModEntry.Sanitize(id);
            if (ModEntry.Animals.ContainsKey(id))
                return;
            ModEntry.Animals.Add(id, new List<AnimalSkin>());
            if (hasBaby)
                this.RegisterAnimalType("Baby"+id, false);
            if (canShear)
                this.RegisterAnimalType("Sheared"+id, false);
        }
    }
}
