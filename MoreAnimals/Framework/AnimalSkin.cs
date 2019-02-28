using System;

namespace Entoarox.MorePetsAndAnimals.Framework
{
    /// <summary>An animal skin.</summary>
    internal class AnimalSkin
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The animal type.</summary>
        public string AnimalType { get; }

        /// <summary>A unique skin ID for the animal type.</summary>
        public int ID { get; }

        /// <summary>The internal asset key.</summary>
        public string AssetKey { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="animalType">The animal type.</param>
        /// <param name="id">A unique skin ID for the animal type.</param>
        /// <param name="assetKey">The internal asset key.</param>
        public AnimalSkin(string animalType, int id, string assetKey)
        {
            this.AnimalType = animalType;
            this.ID = id;
            this.AssetKey = assetKey;
        }
    }
}
