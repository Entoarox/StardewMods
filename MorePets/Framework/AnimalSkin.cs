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
        public AnimalType AnimalType { get; }

        /// <summary>A unique skin ID for the animal type.</summary>
        public int ID { get; }

        /// <summary>The asset key.</summary>
        public string AssetName { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="animalType">The animal type.</param>
        /// <param name="id">A unique skin ID for the animal type.</param>
        /// <param name="assetName">The asset key.</param>
        public AnimalSkin(AnimalType animalType, int id, string assetName)
        {
            this.AnimalType = animalType;
            this.ID = id;
            this.AssetName = assetName;
        }

        public static bool TryParseType(string raw, out AnimalType type)
        {
            raw = raw?.Replace(" ", ""); // convert names like 'Brown Cow'
            return Enum.TryParse(raw, true, out type);
        }
    }
}
