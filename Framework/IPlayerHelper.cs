using System.Collections.Generic;
using StardewValley;
using StardewValley.Characters;

namespace Entoarox.Framework
{
    public interface IPlayerHelper
    {
        /// <summary>
        /// A list-like structure that enables mods to apply modifiers to the player
        /// These modifiers can affect properties such as running and walk speed, glow range, magnet range, or even knockback dealt
        /// See <see cref="PlayerModifier"/> for a full list of all player properties that can be modified this way
        /// </summary>
        IPlayerModifierHelper Modifiers { get; }
        /// <summary>
        /// Moves the player to the given position in the current location
        /// </summary>
        /// <param name="x">The X position to move the player to</param>
        /// <param name="y">The Y position to move the player to</param>
        void MoveTo(int x, int y);
        /// <summary>
        /// Moves the player to the given position in the given static location
        /// </summary>
        /// <param name="location">The name of the static location to move the player to</param>
        /// <param name="x">The X position to move the player to</param>
        /// <param name="y">The Y position to move the player to</param>
        /// <exception cref="ArgumentNullException">Thrown if the given location name can not be resolved as a static location</exception>
        void MoveTo(string location, int x, int y);
        /// <summary>
        /// Moves the player to the given position in the given location
        /// </summary>
        /// <param name="location">A reference to the location to move the player to</param>
        /// <param name="x">The X position to move the player to</param>
        /// <param name="y">The Y position to move the player to</param>
        void MoveTo(GameLocation location, int x, int y);
        /// <summary>
        /// Used to check if the farmer has any pet, with the option to only check for the vanilla pet
        /// </summary>
        /// <param name="onlyVanilla">When true, only the vanilla pet is looked for</param>
        /// <returns>If the farmer has a pet</returns>
        bool HasPet(bool onlyVanilla = false);
        /// <summary>
        /// Used to get a reference to the farmers pet, when multiple pets are found, it tries to identify the vanilla pet and prefers returning that
        /// </summary>
        /// <returns>A <see cref="Pet"/> instance</returns>
        Pet GetPet();
        /// <summary>
        /// Returns a list of all pets found
        /// </summary>
        /// <returns>A list of all pets found in the game</returns>
        List<Pet> GetAllPets();
    }
}
