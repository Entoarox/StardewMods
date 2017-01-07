using System;
using System.Collections.Generic;

using xTile.ObjectModel;

using Microsoft.Xna.Framework;

using StardewValley;

namespace Entoarox.Framework
{
    /**
     * <summary>Enables invoking of the base <see cref="Microsoft.Xna.Framework.Content.ContentManager.Load{T}(string)"/> method</summary>
     */
    public delegate T LoadBase<T>(string assetName);
    /**
     * <summary>The delegate type required by <see cref="IContentRegistry.RegisterHandler{T}(string, FileLoadMethod{T})"/></summary>
     */
    public delegate T FileLoadMethod<T>(LoadBase<T> loadBase, string assetName);

    /// <summary>
    /// Enables simple message display, designed by Kithi and included into the framework with their permission
    /// </summary>
    public interface IMessageBox
    {
        /**
         * <summary>Adds a new message prefixed with a name to the message box</summary>
         * <param name="message">The message to display</param>
         * <param name="name">The prefixed name to use, should usually be the name of your mod</param>
         * <param name="color">What color to use, leave empty to use the default black color</param>
         */
        void receiveMessage(string message, string name, Color color = new Color());
        /**
         * <summary>Adds a new message without any prefix to the message box</summary>
         * <param name="message">The message to display</param>
         * <param name="color">What color to use, leave empty to use the default black color</param>
         */
        void receiveMessage(string message, Color color = new Color());
    }
    /**
     * <summary>Contains EntoFramework methods related to affecting the player</summary>
     */
    public interface IPlayerHelper
    {
        /**
         * <summary>Adds a new <see cref="FarmerModifier"/> that will remain in effect on the player</summary>
         * <param name="modifier">The modifier to add to the player</param>
         */
        void AddModifier(FarmerModifier modifier);
        /**
         * <summary>Removes the <see cref="FarmerModifier"/> from affecting the player, must be a pre-existing modifier</summary>
         * <param name="modifier">The pre-existing modifier to remove from the player</param>
         */
        void RemoveModifier(FarmerModifier modifier);
        /**
         * <summary>Checks if the given <see cref="FarmerModifier"/> is currently affecting the player, must be a pre-existing modifier</summary>
         * <param name="modifier">The pre-existing modifier to check for application</param>
         */
        bool HasModifier(FarmerModifier modifier);
    }
    /**
     * <summary>Contains EntoFramework methods related to the serializer</summary>
     */
    public interface ITypeRegistry
    {
        /**
         * <summary>Get a list of all custom types that are registered for injection into the stardew serializer</summary>
         * <returns>A list of the types that have been added using <see cref="RegisterType{T}"/> to the serializer</returns>
         */
        List<Type> GetInjectedTypes();
        /**
         * <summary>Registers a new <see cref="Type"/> that will be injected into the stardew serializer</summary>
         * <typeparam name="T">The <see cref="Type"/> to add to the serializer</typeparam>
         */
        void RegisterType<T>();
    }
    /**
     * <summary>Contains EntoFramework methods related to the content manager</summary>
     */
    public interface IContentRegistry
    {
        /**
         * <summary>Redirects all attempts to load a specific <see cref="Microsoft.Xna.Framework.Graphics.Texture2D"/> to the given file</summary>
         * <param name="file">The full path to the replacement texture file, with extension</param>
         * <param name="key">The path used in <see cref="Microsoft.Xna.Framework.Content.ContentManager.Load{T}(string)"/> to load the file</param>
         */
        void RegisterTexture(string key, string file);
        /**
         * <summary>Redirects all attempts to load the given asset to the new path</summary>
         * <param name="file">The full path to the replacement xnb file, without extension</param>
         * <param name="key">The path used in <see cref="Microsoft.Xna.Framework.Content.ContentManager.Load{T}(string)"/> to load the file</param>
         */
        void RegisterXnb(string key, string file);
        /**
         * <summary>Redirects all attempts to load the given asset to the given delegate</summary>
         * <param name="method">The delegate that should be invoked</param>
         * <param name="key">The path used in <see cref="Microsoft.Xna.Framework.Content.ContentManager.Load{T}(string)"/> to load the file</param>
         */
        void RegisterHandler<T>(string key, FileLoadMethod<T> method);
    }
    /**
     * <summary>Contains EntoFramework methods to simplify operations to game locations</summary>
     */
    [Obsolete("Use the extension methods found in Entoarox.Framework.Extensions.GameLocationExtensions instead")]
    public interface ILocationHelper
    {
        /**
         * <summary>Sets a specific tile to a specific tile index</summary>
         * <param name="location">The location to target, as either a <see cref="string"/> or <see cref="GameLocation"/></param>
         * <param name="layer">The name of the layer the tile you wish to edit is on</param>
         * <param name="x">The X position of the tile you wish to edit</param>
         * <param name="y">The Y position of the tile you wish to edit</param>
         * <param name="index">The index on the tilesheet you wish for the tile to have</param>
         * <param name="sheet">The Id of the tilesheet, if not set the existing tilesheet is kept</param>
         */
        void SetStaticTile(LocationReference location, string layer, int x, int y, int index, string sheet = null);
        /**
         * <summary>Sets a specific tile to a animated collection of tile indexes</summary>
         * <param name="location">The location to target, as either a <see cref="string"/> or <see cref="GameLocation"/></param>
         * <param name="layer">The name of the layer the tile you wish to edit is on</param>
         * <param name="x">The X position of the tile you wish to edit</param>
         * <param name="y">The Y position of the tile you wish to edit</param>
         * <param name="indexes">The list of indexes on the tilesheet you wish for the tile to animate through</param>
         * <param name="interval">How quickly you wish for the tile to switch between indexes</param>
         * <param name="sheet">The Id of the tilesheet, if not set the existing tilesheet is kept</param>
         */
        void SetAnimatedTile(LocationReference location, string layer, int x, int y, int[] indexes, int interval, string sheet = null);
        /**
         * <summary>Sets the property value of a specific property on a specific tile</summary>
         * <param name="location">The location to target, as either a <see cref="string"/> or <see cref="GameLocation"/></param>
         * <param name="layer">The name of the layer the tile you wish to edit is on</param>
         * <param name="x">The X position of the tile you wish to edit</param>
         * <param name="y">The Y position of the tile you wish to edit</param>
         * <param name="key">The key of the property you wish to set</param>
         * <param name="value">The value you wish to set it to, must be valid as a <see cref="PropertyValue"/></param>
         */
        void SetTileProperty(LocationReference location, string layer, int x, int y, string key, PropertyValue value);
        /**
         * <summary>Gets the property value of a specific property on a specific tile</summary>
         * <param name="location">The location to target, as either a <see cref="string"/> or <see cref="GameLocation"/></param>
         * <param name="layer">The name of the layer the tile you wish to edit is on</param>
         * <param name="x">The X position of the tile you wish to edit</param>
         * <param name="y">The Y position of the tile you wish to edit</param>
         * <param name="key">The key of the property you wish to get</param>
         * <returns>The value of the property or <see cref="null"/> if not set</returns>
         */
        PropertyValue GetTileProperty(LocationReference location, string layer, int x, int y, string key);
        /**
         * <summary>Removes the tile at the given position</summary>
         * <param name="location">The location to edit, as either a <see cref="string"/> or <see cref="GameLocation"/></param>
         * <param name="layer">The name of the layer the tile you wish to edit is on</param>
         * <param name="x">The X position of the tile you wish to edit</param>
         * <param name="y">The Y position of the tile you wish to edit</param>
         */
        void RemoveTile(LocationReference location, string layer, int x, int y);
        /**
         * <summary>Sets the property value of a specific property on the given tilesheet</summary>
         * <param name="location">The location to target, as either a <see cref="string"/> or <see cref="GameLocation"/></param>
         * <param name="sheet">The Id of the tilesheet you wish to set the property for</param>
         * <param name="key">The key of the property you wish to set</param>
         * <param name="value">The value you wish to set it to, must be valid as a <see cref="PropertyValue"/></param>
         */
        void SetTilesheetProperty(LocationReference location, string sheet, string key, PropertyValue value);
        /**
         * <summary>Gets the property value of a specific property on the given tilesheet</summary>
         * <param name="location">The location to target, as either a <see cref="string"/> or <see cref="GameLocation"/></param>
         * <param name="sheet">The Id of the tilesheet you wish to get the property of</param>
         * <param name="key">The key of the property you wish to set</param>
         * <returns>The value of the property or <see cref="null"/> if not set</returns>
         */
        PropertyValue GetTilesheetProperty(LocationReference location, string sheet, string key);
    }
    /**
     * <summary>Contains all methods that interact with the Custom Condition System build into the framework</summary>
     */
    interface IConditionHandler
    {
        bool CheckCondition(string condition);
        bool CheckConditionList(string conditionlist, char seperator = ',', int limit = 5);
        /**
         * <summary>Tries to find and report issues inside condition lists</summary>
         * <remarks>
         * The conflict resolution is not perfect as it had to be hand-written to deal with a lot of situations
         * Further, some of the newer conditions have not been written into the conflict resolution yet and thus will not cause issues
         * Some other conflicts are also difficult to actually detect, so the method at this time does not handle them
         * </remarks>
         * <param name="conditions">The list to check for conflicts</param>
         * <param name="seperator">The seperator used in this specific condition list</param>
         * <param name="limit">The maximum amount of conditions allowed in the list</param>
         * <param name="strict">Setting strict mode to true means that issues that do not effect the validity of a list, but do affect its efficiency are also resolved</param>
         * <returns>A string representing the found conflict or null if no conflict was found</returns>
         */
        string FindConflictingConditions(string conditions, char seperator = ',', int limit = 5, bool strict = true);
    }
    /**
     * <summary>Defines modifiers that can be applied to the player</summary>
     */
    public class FarmerModifier
    {
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public int MagnetRange = 0;

        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public int WalkSpeedModifier = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public int RunSpeedModifier = 0;

        /**
         * <summary>Not additive, largest modifier used</summary>
         */
        public float GlowDistance = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float StaminaRegenModifier = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float HealthRegenModifier = 0;

        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float attackIncreaseModifier = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float KnockbackModifier = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float CritChanceModifier = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float CritPowerModifier = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float WeaponSpeedModifier = 0;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float WeaponPrecisionModifier = 0;

        private float _Farming = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float ExperienceModifierFarming { get { return _Farming; } set { _Farming = Math.Max(-1f, Math.Min(5f, value)); } }
        private float _Fishing = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float ExperienceModifierFishing { get { return _Fishing; } set { _Fishing = Math.Max(-1f, Math.Min(5f, value)); } }
        private float _Foraging = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float ExperienceModifierForaging { get { return _Foraging; } set { _Foraging = Math.Max(-1f, Math.Min(5f, value)); } }
        private float _Mining = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float ExperienceModifierMining { get { return _Mining; } set { _Mining = Math.Max(-1f, Math.Min(5f, value)); } }
        private float _Combat = 1;
        /**
         * <summary>Additive, multiple modifiers will stack</summary>
         */
        public float ExperienceModifierCombat { get { return _Combat; } set { _Combat = Math.Max(-1f, Math.Min(5f, value)); } }
    }
    /**
     * <summary>
     * A <see cref="GameLocation"/> or a <see cref="string"/> used in <see cref="StardewValley.Game1.getLocationFromName(string)"/>
     * <para>Has implicit conversion, so simply using a GameLocation or string as input will work
     * </para></summary>
     */
    public class LocationReference
    {
        private GameLocation Value;
        private LocationReference(GameLocation value)
        {
            if (value == null)
                throw new ArgumentNullException("location");
            Value = value;
        }
        public static implicit operator LocationReference(GameLocation value)
        {
            return new LocationReference(value);
        }
        public static implicit operator LocationReference(string value)
        {
            return new LocationReference(Game1.getLocationFromName(value));
        }
        public static implicit operator GameLocation(LocationReference value)
        {
            return value.Value;
        }
    }
}
