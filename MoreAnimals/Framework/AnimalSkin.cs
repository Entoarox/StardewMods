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

        /// <summary>The internal asset key.</summary>
        public string AssetKey { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="animalType">The animal type.</param>
        /// <param name="id">A unique skin ID for the animal type.</param>
        /// <param name="assetKey">The internal asset key.</param>
        public AnimalSkin(AnimalType animalType, int id, string assetKey)
        {
            this.AnimalType = animalType;
            this.ID = id;
            this.AssetKey = assetKey;
        }

        /// <summary>Try to parse a raw animal type (e.g. from a filename) into an <see cref="AnimalType"/> value.</summary>
        /// <param name="raw">The raw animal type.</param>
        /// <param name="type">The parsed value, if valid.</param>
        /// <returns>Returns whether the value was successfully parsed.</returns>
        public static bool TryParseType(string raw, out AnimalType type)
        {
            raw = raw?.Replace(" ", ""); // convert names like 'Brown Cow'
            return Enum.TryParse(raw, true, out type);
        }
    }
}
